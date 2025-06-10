
# Project Description: VoidMail API

**VoidMail** is a lightweight, backend API service built with **ASP.NET Core**. Its primary function is to send emails through a simple, secure, and scalable RESTful interface. The project is designed with a clean architecture, separating concerns into distinct layers for controllers, core business logic, and services. It leverages Microsoft Azure for its core functionalities, including email delivery and secrets management, and is set up for automated deployment to the cloud. The API is well-documented using Swagger (OpenAPI), making it easy for developers to understand and integrate with.

---

## How It Works

The service operates through a single API endpoint, which processes requests to send emails. The workflow is as follows:

1. **API Endpoint**: The application exposes a `POST /Mail/send` endpoint, which is defined in the `MailController`.
2. **Request Handling**: The endpoint accepts a JSON object (`EmailDto`) containing the recipient's address, subject, HTML body, and an optional plain-text body. The request data is validated to ensure all required fields are present and correctly formatted.
3. **Service Layer**: If the request is valid, the `MailController` calls the `SendEmailEventAsync` method from the `MailService`. This service is injected into the controller using the Dependency Injection pattern, which is configured in `Core/DependencyInjection.cs`.
4. **Email Delivery**: The `MailService` is responsible for the actual email sending logic. It retrieves the Azure Communication Services connection string and the sender's email address from the application's configuration. It then uses the `EmailClient` from the Azure SDK to construct and send the email.
5. **Configuration & Secrets**: The application is configured to use **Azure Key Vault** to manage sensitive information like connection strings, as specified in `Program.cs` and `appsettings.json`. For local development, it uses the settings in `launchSettings.json`.
6. **Response Handling**: The API provides clear, standardized responses using a custom `ApiResponseHelper`. If the email is sent successfully, it returns a `200 OK` status. If there are validation issues or server-side errors, it returns a structured problem details response (e.g., `400 Bad Request` or `500 Internal Server Error`).
7. **Deployment**: The project includes a **GitHub Actions** workflow (`main_voidmail.yml`) that automates the entire CI/CD process. When code is pushed to the `main` branch, the workflow automatically builds the .NET project, publishes the application artifact, and deploys it to a production slot on an **Azure Web App** named "VoidMail".

---

## Tech Stack

Here is a summary of the technologies and services used in this project:

* **Backend Framework**: ASP.NET Core
* **Programming Language**: C#
* **Cloud Platform**: Microsoft Azure
  * **Azure Communication Services**: Used for sending emails.
  * **Azure Key Vault**: For secure management of application secrets.
  * **Azure Web App**: For hosting the application in the cloud.
* **API & Documentation**:
  * RESTful API design principles.
  * **Swagger (OpenAPI)**: For generating interactive API documentation.
  * **Scalar**: Used for the API reference UI in the development environment.
* **DevOps**:
  * **Git & GitHub**: For version control.
  * **GitHub Actions**: For Continuous Integration & Continuous Deployment (CI/CD).
* **Architecture & Patterns**:
  * Dependency Injection (DI) using the Composition Root pattern.
  * Separation of Concerns (Controllers, Services, DTOs).
  * Centralized API Response Handling.
