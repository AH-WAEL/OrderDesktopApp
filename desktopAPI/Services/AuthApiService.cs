using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using desktopAPI.Models;
using System.Runtime.CompilerServices;

namespace desktopAPI.Services
{
    internal class AuthApiService
    {

        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly string AuthApiBaseUrl = "http://localhost:50000/api/users";

        private static int _tokenCheckErrorCount = 0;

        private static DateTime _lastExpiryWarningShown = DateTime.MinValue;


        //action and action<bool> are delegates for token events
        public static event Action TokenExpired;

        public static event Action<bool> TokenRefreshed;

        public static string AccessToken { get; private set; }
        public static string RefreshToken { get; private set; }
        public static Guid UserId { get; private set; }
        public static string Username { get; private set; }
        public static string Role { get; private set; }
        public static string Email { get; private set; }
        public static DateTime AccessTokenExpiry { get; private set; }

        static AuthApiService()
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public static void SetTokens(string accessToken, string refreshToken, DateTime refreshTokenExpiryFromDb)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;

            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(accessToken);

                    Username = token.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                    var userIdStr = token.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    Role = token.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                    Email = token.Claims.First(x => x.Type == ClaimTypes.Email).Value;

                    AccessTokenExpiry = token.ValidTo.ToLocalTime();

                    if (Guid.TryParse(userIdStr, out Guid userId))
                    {
                        UserId = userId;
                    }

                    Console.WriteLine($"Authentication successful - User: {Username}, Role: {Role}");
                    Console.WriteLine($"Access token expires: {AccessTokenExpiry:yyyy-MM-dd HH:mm:ss}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token parsing failed: {ex.Message}");
                    ClearTokens();
                }
            }
        }

        public static void ClearTokens()
        {
            AccessToken = null;
            RefreshToken = null;
            UserId = Guid.Empty;
            Username = null;
            Role = null;
            Email = null;
            AccessTokenExpiry = DateTime.MinValue;

            Console.WriteLine("Authentication tokens cleared");
            // Keep session state in sync with cleared tokens
        }

        public static bool IsAccessTokenValid()
        {
            return !string.IsNullOrEmpty(AccessToken) && DateTime.Now < AccessTokenExpiry.AddMinutes(-1);
        }


        public static async Task<bool> CheckTokensAndRefresh(Form mainform)
        {
            try
            {
                var dbValidation = await ValidateRefreshTokenWithDatabase();

                if (!dbValidation.IsValid)
                {
                    Console.WriteLine($"Session terminated - {dbValidation.Message}");
                    // Do NOT clear tokens here; allow Logout to invalidate refresh token on server
                    //TokenExpired?.Invoke();
                    await LogoutAsync(mainform).ConfigureAwait(false);
                    return false;
                }

                var timeUntilExpiry = AccessTokenExpiry - DateTime.Now;

                if (timeUntilExpiry.TotalMinutes <= 1 || !IsAccessTokenValid())
                {
                    Console.WriteLine("Refreshing access token...");

                    var success = await RefreshTokenAsync();

                    if (!success)
                    {
                        Console.WriteLine("Access token refresh failed - Session terminated");
                        await LogoutAsync(mainform);
                         return false;
                    }
                    else
                    {
                        Console.WriteLine("Access token refreshed successfully");
                    }
                }

                var refreshTimeLeft = dbValidation.DatabaseTime - DateTime.Now;
                _tokenCheckErrorCount = 0;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation error: {ex.Message}");

                _tokenCheckErrorCount++;
                if (_tokenCheckErrorCount >= 3)
                {
                    Console.WriteLine("Multiple authentication failures - Session terminated");

                    await LogoutAsync(mainform);

                }
                return false;
            }
        }

        public static async Task<bool> RefreshTokenAsync()
        {
            try
            {
                var refreshRequest = new RefreshTokenRequestDto
                {
                    UserId = UserId,
                    RefreshToken = RefreshToken
                };

                var json = JsonConvert.SerializeObject(refreshRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{AuthApiBaseUrl}/refresh-token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponseDto>(responseContent);

                    _tokenCheckErrorCount = 0;
                    SetTokens(tokenResponse.AccessToken, tokenResponse.RefreshToken, tokenResponse.RefreshTokenExpiryTime);

                    return IsAccessTokenValid();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Token refresh failed - Status: {response.StatusCode}, Error: {error}");

                    return IsAccessTokenValid();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token refresh exception: {ex.Message}");
                return false;
            }
        }

        public static async Task LogoutAsync(Form mainform)
        {
            try
            {
                if (string.IsNullOrEmpty(RefreshToken) || UserId == Guid.Empty)
                {
                    Console.WriteLine("No active session to logout");
                }

                Console.WriteLine("Logging out - Invalidating refresh token in database...");

                var logoutRequest = new RefreshTokenRequestDto
                {
                    UserId = UserId,
                    RefreshToken = RefreshToken
                };

                var json = JsonConvert.SerializeObject(logoutRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{AuthApiBaseUrl}/logout", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    Console.WriteLine($"Database logout successful: {result.Message}");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Database logout failed - Status: {response.StatusCode}, Error: {error}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
                MessageBox.Show($"An error occurred during logout: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearTokens();
            }
            finally
            {
                ClearTokens();
                CloseAndShowLogin(mainform);
            }
        }

        private static void CloseAndShowLogin(Form mainform)
        {
            void perform()
            {
                // Prefer hiding the current form to keep the message loop alive
                var host = mainform ?? Application.OpenForms.Cast<Form>().FirstOrDefault();
                if (host != null && !host.IsDisposed)
                {
                    try { host.Hide(); } catch { }
                }

                // Close other open forms (except the hidden host and any Login)
                try
                {
                    var open = Application.OpenForms.Cast<Form>().ToArray();
                    foreach (var f in open)
                    {
                        if (f != host && f is not Login && !f.IsDisposed)
                        {
                            try { f.Close(); } catch { }
                        }
                    }
                }
                catch { }

                // Show or activate Login
                var login = Application.OpenForms.Cast<Form>().OfType<Login>().FirstOrDefault();
                if (login == null || login.IsDisposed)
                {
                    login = new Login { StartPosition = FormStartPosition.CenterScreen };
                    login.Show(); // non-modal so the message loop continues
                }
                else
                {
                    if (!login.Visible) login.Show();
                    login.Activate();
                }
            }

            try
            {
                var target = mainform ?? Application.OpenForms.Cast<Form>().FirstOrDefault();
                if (target != null && !target.IsDisposed && target.InvokeRequired)
                    target.BeginInvoke((Action)(() => perform()));
                else
                    perform();
            }
            catch
            {
                perform();
            }
        }
    

        public static async Task<RefreshTokenValidationDto> ValidateRefreshTokenWithDatabase()
        {
            try
            {
                if (string.IsNullOrEmpty(RefreshToken) || UserId == Guid.Empty)
                {
                    return new RefreshTokenValidationDto
                    {
                        IsValid = false,
                        Message = "No refresh token available",
                        DatabaseTime = DateTime.UtcNow
                    };
                }

                var validationRequest = new RefreshTokenRequestDto
                {
                    UserId = UserId,
                    RefreshToken = RefreshToken
                };

                var json = JsonConvert.SerializeObject(validationRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{AuthApiBaseUrl}/validate-refresh-token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var validation = JsonConvert.DeserializeObject<RefreshTokenValidationDto>(responseContent);
                    return validation;
                }
                else
                {
                    return new RefreshTokenValidationDto
                    {
                        IsValid = false,
                        Message = $"API error: {response.StatusCode}",
                        DatabaseTime = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                return new RefreshTokenValidationDto
                {
                    IsValid = false,
                    Message = $"Validation exception: {ex.Message}",
                    DatabaseTime = DateTime.UtcNow
                };
            }
        }
    }
}

 
