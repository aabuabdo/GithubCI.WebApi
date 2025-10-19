# GitHub Actions + Self-Hosted Runner + IIS + .NET Deployment

## 1️⃣ إنشاء Azure VM

```powershell
$rg = ""                # اسم Resource Group
$vmname = ""    # اسم الـ VM
$location = ""     # المنطقة الجغرافية
$adminuser = "azureadmin"    # اسم المستخدم الإداري
$adminpass = "P@ssw0rd12345!" # كلمة المرور للإدارة


az vm create `
  --resource-group $rg `
  --name $vmname `
  --image Win2022Datacenter `
  --admin-username $adminuser `
  --admin-password $adminpass `
  --size Standard_B2s `
  --public-ip-sku Standard `
  --location $location
```

---

## 2️⃣ الاتصال بالـ VM

- استخدم **RDP** من Azure Portal
- اسم المستخدم: `azureadmin`
- كلمة المرور: `P@ssw0rd12345!`

---

## 3️⃣ تثبيت IIS على الـ VM

```powershell
Install-WindowsFeature -name Web-Server -IncludeManagementTools
```

---

## 4️⃣ تثبيت .NET 8 Hosting Bundle

1. حمّل **Hosting Bundle** من: [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. ثبّته على الـ VM
3. أعد تشغيل IIS:

```powershell
iisreset
```

---

## 5️⃣ إعداد GitHub Self-Hosted Runner

```powershell
cd C:\actions-runner
.\config.cmd --url https://github.com/<username>/<repo> --token <NEW_TOKEN>
.\svc install
.\svc start
```

---

## 6️⃣ نشر تطبيق ASP.NET Core على IIS

```powershell
Import-Module WebAdministration

$siteName = "MyApp"
$physicalPath = "C:\inetpub\wwwroot\MyApp"
$appPoolName = "MyAppPool"

if (-not (Test-Path $physicalPath)) {
    New-Item -Path $physicalPath -ItemType Directory
}

if (-not (Test-Path "IIS:\AppPools\$appPoolName")) {
    New-Item IIS:\AppPools\$appPoolName
}

if (-not (Get-ChildItem IIS:\Sites | Where-Object { $_.Name -eq $siteName })) {
    New-Item -Path "IIS:\Sites\$siteName" -physicalPath $physicalPath -bindings @{protocol="http";bindingInformation="*:8080:"}
    Set-ItemProperty "IIS:\Sites\$siteName" -Name applicationPool -Value $appPoolName
}

Remove-Item -Path "$physicalPath\*" -Recurse -Force
Copy-Item -Path .\publish\* -Destination $physicalPath -Recurse -Force

Restart-WebAppPool $appPoolName
```

---

## 7️⃣ ملاحظات هامة

- تأكد أن **Self-Hosted Runner يعمل كـ Administrator**
- App Pool يجب أن يكون له أذونات **Read & Execute** على مجلد النشر
- يمكنك تعديل المنفذ من `8080` إلى أي منفذ آخر
- تحقق من التطبيق عبر: `http://<VM_PUBLIC_IP>:8080/swagger/index.html`
