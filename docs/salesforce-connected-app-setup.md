# How to Create a Connected App (External Client App) in Salesforce

> Note: In current Salesforce releases, "Connected Apps" have been renamed to
> "External Client Apps". The creation flow described below reflects this
> updated interface, as observed in the org's Setup menu.

## Steps

### 1. Navigate to the App Manager
- Go to **Setup**.
- In the Quick Find box, search for **App Manager** (under *Platform Tools > Apps*).

### 2. Start creating a new app
- Click **New External Client App** (this is the current equivalent of the classic "New Connected App" button).

### 3. Fill in Basic Information
- **External Client App Name** (required)
- **API Name** (required, auto-generated from the app name, editable)
- **Contact Email** (required)
- **Distribution State** (required, e.g. "Local")
- Optional fields: Contact Phone, Info URL, Logo Image URL, Icon URL, Description

### 4. Configure API (OAuth Settings)
- Expand the **API (Enable OAuth Settings)** section.
- Check **Enable OAuth**.
- Enter the **Callback URL** (required) — the redirect URI used after OAuth authorization:
  ```
  https://localhost:7257/api/salesforce/callback
  ```
- Select the following **OAuth Scopes** and move them to "Selected OAuth Scopes":
  - Manage user data via APIs (api)
  - Perform requests at any time (refresh_token, offline_access)
  - Access unique user identifiers (openid)

### 5. Enable OAuth Flows (Flow Enablement)
Enable only:
- **Enable Authorization Code and Credentials Flow**

(Leave Client Credentials Flow, Device Flow, JWT Bearer Flow, and Token Exchange Flow unchecked.)

### 6. Configure Security settings
- In the **Security** section, uncheck **Require Proof Key for Code Exchange (PKCE) extension for Supported Authorization Flows**.

### 7. (Optional) Additional sections
- **Web App (Enable SAML Settings)** — for SAML-based single sign-on.
- **Canvas App Settings** — if building a Canvas app.

### 8. Save the app
- Click **Create**.
- Salesforce will generate the **Consumer Key** and **Consumer Secret**, which you'll need for your integration.

## Where to find the Consumer Key and Consumer Secret
- Go to **Setup > Platform Tools > Apps > External Client Apps > External Client App Manager**.
- Click the app you created (e.g. from the list).
- Open the **Settings** tab.
- Expand the **OAuth Settings** section.
- Click the **Consumer Key and Secret** button.
- A new page opens showing the **Consumer Key** and **Consumer Secret**, each with a **Copy** button.
- Note: depending on your org's security settings, you may be prompted to verify your identity (e.g. via an email verification code) before the secret is revealed.

## Updating appsettings.json
Once you have the credentials, update `src/SalesforceTest.Api/appsettings.json`:

```json
"Salesforce": {
  "ClientId": "<Consumer Key>",
  "ClientSecret": "<Consumer Secret>",
  "RedirectUri": "https://localhost:7257/api/salesforce/callback",
  "AuthBaseUrl": "https://login.salesforce.com"
}
```

Restart the API after saving.
