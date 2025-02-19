# ใช้ .NET SDK เพื่อ Build โปรเจค
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# คัดลอกไฟล์โปรเจคไปยัง Container
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# ใช้ .NET Runtime สำหรับรันโปรเจค
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# กำหนดพอร์ต 7039
EXPOSE 7039

# คำสั่งรันแอป ASP.NET Core โดยกำหนดให้ใช้พอร์ต 7039
CMD ["dotnet", "TheExpertise_Emp.dll", "--urls", "http://0.0.0.0:7039"]
