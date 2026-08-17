Here’s the quick path to do exactly what you asked (Windows + Visual Studio + .NET 8):

## 1) Install Visual Studio
1. Go to: https://visualstudio.microsoft.com/downloads/
2. Download **Visual Studio 2022 Community** (free).
3. Run installer.
4. In **Workloads**, check:
   - **ASP.NET and web development** (if web project)
   - **.NET desktop development** (if desktop app)
5. Click **Install**.

## 2) Install .NET 8 SDK
1. Go to: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
2. Download **.NET SDK 8.x** (not Runtime only).
3. Install it.
4. Verify in terminal (Command Prompt):
```bash
dotnet --version
```
You should see something like `8.0.xxx`.

## 3) Open your project in Visual Studio
1. Open Visual Studio.
2. Click **Open a project or solution**.
3. Select your `.sln` file from `University-Project`.

## 4) Set Startup Project
1. In **Solution Explorer**, right-click the project you want to run.
2. Click **Set as Startup Project**.
3. (Optional) If multiple projects:
   - Right-click solution → **Set Startup Projects...**
   - Choose **Single startup project** or **Multiple startup projects**.

## 5) Restore + Build + Run
1. In Visual Studio: **Build → Build Solution**.
2. If errors about packages: right-click solution → **Restore NuGet Packages**.
3. Run with:
   - **F5** (Run with debugger), or
   - **Ctrl+F5** (Run without debugger).

---

If you want, I can also update your `README.md` in `Rajdip27/University-Project` with a clean **“Installation & Run”** section so anyone can follow these exact steps.
