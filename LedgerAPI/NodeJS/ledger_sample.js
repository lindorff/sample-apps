const ClientOAuth2 = require('client-oauth2')
const https = require('https');
const fs = require('fs');

var oauth2 = new ClientOAuth2({
    clientId: '<your_clientId_here>',
    clientSecret: '<your_clientSecret_here>',
    accessTokenUri: 'https://idp.lowell.io/auth/realms/test-clients/protocol/openid-connect/token',
    scopes: ['openid']
});

oauth2.credentials.getToken()
	.then(function (user) {
		console.log(user.data.access_token);
		
		const options = {
			hostname: 'api.lowell.com',
			path: '/fin/qa/invoice/ledger/v1/customers/1000014/invoices',  
			headers: {
				Authorization: 'Bearer '+user.data.access_token
			}
		}
	
		https.get(options, (response) => {			

			var result = ''
			response.on('data', function (chunk) {
				result += chunk;
			});

			response.on('end', function () {
				console.log(result);
			});			
		});
	});
