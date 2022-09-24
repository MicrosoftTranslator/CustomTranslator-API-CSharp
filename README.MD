While AADv2 doesn’t fully support a non-interactive solution, it does provide long lived refresh tokens so that the id token can be refreshed when needed. 

What this means is a workflow like this can be used for automation scenarios:

1) Log-in once interactively to get the id token and a refresh token.
2) Store both the id token and the refresh token in a cache.
3) Automation can use the id token to login without the need for user intervention.
4) When the id token expires, a new token can be retrieved and stored by using the refresh token
 
The latest version of the Microsoft.Identity.Client libraries contain very nice methods that handle much of this behind the scenes. I created a working code sample (complete with comments to describe what is happening); this VS solution.

One thing to note- This solution depends entirely on the reliability of the refresh tokens issued by AADv2. The documentation contains a section on these special tokens here: https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-id-and-access-tokens#refresh-tokens. Two things seem important here:

Apps must be granted “offline_access” scope. This doesn’t seem to be a problem, but we should keep it in mind
They warn against relying on refresh tokens because they “can be invalidated at any moment for various reasons”. This hasn’t been and shouldn’t be a problem, but your autmation should probably be written such that it sends a notification in case of Sign-In failure/ refresh token invalidation.

To use this solution to generate access token, you need to register an application at the Microsoft Appp Registration portal:

     https://ms.portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade 

1) Click "Register an application" and type app-name and select account type. 
2) Click "Register" which will give you an MSAL "Application (client) ID" that you can use with the authentication APIs. 
3) Click "Authentication" blade and configure "Public client/native (mobile & desktop)" by clicking on the drop down
arrow. Your configuration should look like this:

![Authentication](https://github.com/MicrosoftTranslator/CustomTranslator-API-CSharp/images/Authentication.gif)

Then modify API permissions to look like so (don't foget to grant admin consent)

![API permissions](https://github.com/MicrosoftTranslator/CustomTranslator-API-CSharp/images/api-permissions.gif)


Now you have an "Application (client) ID" value, update the app.config:

 <add key="clientId" value="" /> 

with your value.
