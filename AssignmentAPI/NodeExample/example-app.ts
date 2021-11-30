const axios = require('axios').default;
import { ClientCredentials } from 'simple-oauth2';

process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = '0';

const REALM = 'test_clients';
const CLIENT_ID = 'testclientid';
const CLIENT_SECRET = 'xxxxxx';
const ENVIRONMENT = 'test';

async function sendNewCase() {
  const exampleBody = {
    id: '1234567',
    subclientId: '526731',
    reference: '2057710312563291',
    class: 'SomeClass',
    type: 'Collection',
    descriptionLines: [
      'Target: Address street 23 A 5',
      'Contact person: Erik Eriksson, 04012345678',
      'Termination date: 20.03.2023',
    ],
    debtors: [
      {
        type: 'Private',
        role: 'Main',
        id: '121212-123A',
        firstName: 'Foo',
        lastName: 'Barson',
        companyName: 'SomeCompanyName',
        street: 'Address Street 10 C 3',
        zip: '90200',
        city: 'Turku',
        country: 'FI',
        phone: '040 987654321',
        email: 'foo@bar.test',
        voluntaryCollectionForbidden: true,
      },
    ],
    invoices: [
      {
        invoiceNumber: '100019',
        invoiceDate: '2020-03-25T00:00:00+02:00',
        dueDate: '2020-04-08T00:00:00+03:00',
        receivableType: 'SO',
        openPrincipal: 50,
        interestRate: 'FI_Consumer',
        interestDate: '2020-04-12T00:00:00+03:00',
        reminderDate: '2020-04-10T00:00:00+03:00',
        reminderFee: 15,
        referenceNumber: 1234,
        description: 'Test bill',
        originalInvoice: {
          originalPrincipal: 300,
          lines: [
            {
              description: 'Test',
              amount: 150,
            },
            {
              description: 'Test 2',
              amount: 150,
            },
          ],
        },
      },
    ],
  };

  const sendNewCaseUrl = `https://api.lowell.com/${ENVIRONMENT}/collection/fi/assignment-api/v1/assignments`;

  const client = new ClientCredentials({
    client: {
      id: CLIENT_ID,
      secret: CLIENT_SECRET,
    },
    auth: {
      tokenHost: `https://idp.lowell.io/auth/realms/${REALM}/`,
      tokenPath: 'protocol/openid-connect/token',
    },
  });

  // TODO: Store token in Redis or other cache and re-use
  const tokenResponse = await client.getToken({
    grant_type: 'client_credentials',
  });

  const { access_token: accessToken } = tokenResponse.token;

  const response = await axios(sendNewCaseUrl, {
    method: 'POST',
    mode: 'cors',
    cache: 'no-cache',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    body: JSON.stringify(exampleBody),
  })
    .then((response: any) => response.json())
    .then((json: any) => json)
    .catch((e: any) => {
      console.error('Response data:', e.response.data);
      throw e;
    });

  return response;
}

sendNewCase().then((response) => console.log(response));
