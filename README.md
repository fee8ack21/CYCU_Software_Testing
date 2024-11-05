<a id="readme-top"></a>

# CYCU Software Testing

此專案為 中原大學 軟體測試與漏洞分析 課程測試對象源碼，內容為基於 JWT 的登入驗證功能。

## 專案介紹

### 使用技術

![csharp][csharp-badge]
![dotnet][dotnet-badge]
![postgres][postgres-badge]
![swagger][swagger-badge]
![docker][docker-badge]
![sonarqube][sonarqube-badge]
![git][git-badge]

## 啟動專案

### Postgres

```sh
# 運行 Postgres 容器
docker run --name cycu-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### API

```sh
# 建置 Docker 鏡像
docker build -t cycu-software-testing-api -f ./App.PL/Dockerfile .
# 運行 API 容器
docker run -d -p 7299:80 --name cycu-software-testing-api cycu-software-testing-api
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Git Commit 規範

- feat -> feature
- fix -> bug fix
- docs -> documentation
- style -> formatting, lint stuff
- refactor -> code restructure without changing external behavior
- perf -> performance

<p align="right">(<a href="#readme-top">back to top</a>)</p>

[swagger-badge]: https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=white
[dotnet-badge]: https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[postgres-badge]: https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white
[docker-badge]: https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white
[sonarqube-badge]: https://img.shields.io/badge/Sonarqube-5190cf?style=for-the-badge&logo=sonarqube&logoColor=white
[csharp-badge]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white
[git-badge]: https://img.shields.io/badge/GIT-E44C30?style=for-the-badge&logo=git&logoColor=white