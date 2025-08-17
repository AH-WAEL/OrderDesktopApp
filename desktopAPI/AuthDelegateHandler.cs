using desktopAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


public class AuthDelegateHandler : DelegatingHandler
{
        private readonly WeakReference<Form> _formRef;

    // Use a provider so each form can supply itself: new AuthDelegateHandler(() => this)
    public AuthDelegateHandler(Form form)
    {
        _formRef = new WeakReference<Form>(form);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {

        Form form = null;
        _formRef?.TryGetTarget(out form);

        var ok = await AuthApiService.CheckTokensAndRefresh(form).ConfigureAwait(false);
        if (!ok)
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
            {
                ReasonPhrase = "Token invalid or expired"
            };
        }
        
        if (!string.IsNullOrEmpty(AuthApiService.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthApiService.AccessToken);

        // Call the inner handler to send the request
        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}
