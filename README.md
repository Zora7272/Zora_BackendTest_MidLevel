# WebApplication1

## 專案簡介
本專案為基於 .NET 8 的 ASP.NET Core Web API，整合 Entity Framework Core 與 SQL Server，並使用 Swagger 產生 API 文件。

## 技術架構
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8 (含 SQL Server 支援)
- Swashbuckle.AspNetCore (Swagger)

## 安裝與執行

1. **安裝依賴套件**
   ```bash
   dotnet restore
   ```

2. **資料庫設定**
   - 請於 `appsettings.json` 設定連線字串。
   - 使用 EF Core Migration 建立資料表：
     ```bash
     dotnet ef database update
     ```

3. **啟動專案**
   ```bash
   dotnet run
   ```

4. **API 文件**
   - 啟動後可於 `/swagger` 路徑瀏覽 API 文件與測試介面。

## 專案結構
- `Controllers/`：API 控制器
- `Models/`：資料模型
- `Data/`：資料庫上下文
- `Services/`：業務邏輯服務
- `Swagger/`：Swagger 設定與過濾器

## 主要功能
- CRUD 操作範例
- Swagger 文件自動產生
- 資料庫操作與服務層分離

