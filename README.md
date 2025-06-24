# EpamKse.GameStore

## Prerequisites

Before running the project, ensure you have:
- A **Dockerized Microsoft SQL Server instance**, running and accessible
- .NET 8 SDK
- Git
- Python 3 — for secret key generation

---


### 1. **Clone the Repository**

```bash
git clone git@github.com:sergiiborodavko-epam/epam-kse-game-store.git
cd epam-kse-game-store
cd EpamKse.GameStore.Api
```

### 2. **Create Environment Configuration**

Create a `.env` file inside the `EpamKse.GameStore.Api` and `EpamKse.GameStore.PaymentService` directories:

```bash
touch .env
```

If you are using Windows:
```cmd
echo. > .env
```
### 3. **Configure Environment Variables**

Add the following variables to your `.env` file in `EpamKse.GameStore.Api`:

```env
CONNECTION_STRING=
ACCESS_TOKEN_SECRET=
REFRESH_TOKEN_SECRET=
LICENSE_ENCRYPTION_KEY=
PAYMENT_SERVICE_API_KEY=
```
Important note: **LICENSE_ENCRYPTION_KEY** is a base64 string. Must be 16 or 32 characters long. In development purposes its fine to set it to a random string of numbers and characters

Add the following variables to your `.env` file in `EpamKse.GameStore.PaymentService`:

```env
PAYMENT_SERVICE_API_KEY=
```

`PAYMENT_SERVICE_API_KEY` must be the same for both services for proper communication

### 4. **Generate JWT Secrets**

For `ACCESS_TOKEN_SECRET` and `REFRESH_TOKEN_SECRET`, you need to generate secure base64 strings.

#### **Option A: Using Python Script**

Create secret.py, paste there the following script and run it:

```python
import secrets
import base64

def generate_signing_secret(num_bytes=64):
    secret_bytes = secrets.token_bytes(num_bytes)
    return base64.urlsafe_b64encode(secret_bytes).decode('utf-8')

if __name__ == "__main__":
    secret = generate_signing_secret()
    print("Generated JWT signing secret:", secret)
```

Run the script twice to generate two different secrets:

#### **Option B: Using other way you know**


### 5. **Configure Database Connection**

Set your `CONNECTION_STRING` using this format:

```env
CONNECTION_STRING=Server=localhost,<Port>;Database=GameStoreDb;User Id=sa;Password=<Password>;Encrypt=False;TrustServerCertificate=True;
```

**Example:**
```env
CONNECTION_STRING=Server=localhost,1433;Database=GameStoreDb;User Id=sa;Password=MyStrongPassword123;Encrypt=False;TrustServerCertificate=True;
```

### 6. **Complete .env File Example**

Your final `.env` file should look like this:

```env
CONNECTION_STRING=Server=localhost,1433;Database=GameStoreDb;User Id=sa;Password=MyStrongPassword123;Encrypt=False;TrustServerCertificate=True;
ACCESS_TOKEN_SECRET=your_generated_base64_secret_here
REFRESH_TOKEN_SECRET=your_different_generated_base64_secret_here
```

---

### 7. Restore dependencies (if not using Rider)

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```


### 8. Run project

#### **Option A: Command Line**
```bash
cd epam-kse-game-store
dotnet run --project EpamKse.GameStore.Api
```

#### **Option B: Rider**
1. Open the project in Rider
2. Click the green **"Run"** button (▶️) in the toolbar

