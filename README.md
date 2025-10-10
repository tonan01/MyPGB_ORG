# HƯỚNG DẪN TRIỂN KHAI VÀ VẬN HÀNH TRÊN VPS

## 1. Tổng quan Dự án

Hệ thống PGB_ORG được xây dựng theo kiến trúc Microservice, triển khai bằng .NET Core 8 và React/Vite, sử dụng PostgreSQL làm cơ sở dữ liệu.

| Thành phần | Công nghệ | Image Docker Hub | Cổng truy cập |
| :--- | :--- | :--- | :--- |
| **Backend (Microservices)** | .NET 8, CQRS/DDD | `tonan01/pgb-auth-api`, v.v. | 7000, 7001, 7002, 7003 |
| **API Gateway** | Ocelot, .NET 8 | `tonan01/pgb-apigateway` | **7000** |
| **Frontend** | React/Vite, Nginx | `tonan01/pgb-frontend` | **5173** |
| **Databases** | PostgreSQL 16 | `postgres:16-alpine` | 5432, 5433, 5434 |

---

## 2. Yêu cầu Tiên quyết trên VPS

Đảm bảo VPS của bạn đã cài đặt và cấu hình các công cụ sau:

1.  **Docker Engine:** Đã cài đặt và đang chạy.
2.  **Docker Compose (V2):** Đã cài đặt (lệnh `docker compose` thay vì `docker-compose`).
3.  **SSH Access:** Có quyền truy cập terminal vào VPS.

---

## 3. Quy trình Triển khai Lên VPS (Production)

Vì images đã được build sẵn và đẩy lên Docker Hub, chúng ta chỉ cần PULL (kéo) về.

### Bước 1: Chuẩn bị Files cấu hình

Tạo một thư mục triển khai mới trên VPS (ví dụ: `~/pgb-deploy`). Bạn chỉ cần copy **HAI** file sau vào thư mục này:

1.  **`docker-compose.yml`**: File cấu hình chứa tất cả 8 Services (DBs, Backend, Frontend) và trỏ đến images `tonan01/...:latest`.
2.  **`.env`**: File chứa các khóa bí mật của hệ thống (`POSTGRES_PASSWORD`, `JWT_SECRETKEY`, `OPENAI_APIKEY`). **Đảm bảo file này được bảo mật nghiêm ngặt.**

### Bước 2: Đăng nhập và Khởi chạy

1.  **Đăng nhập Docker Hub** trên VPS:

    ```bash
    docker login
    # Nhập thông tin đăng nhập của bạn (tonan01)
    ```

2.  **Khởi động hệ thống:** Chạy lệnh `docker compose up` từ thư mục triển khai (nhớ bỏ cờ `--build`):

    ```bash
    # Chuyển vào thư mục chứa docker-compose.yml
    cd ~/pgb-deploy 

    # Kéo và chạy tất cả services ở chế độ nền
    docker compose up -d
    ```
    Docker sẽ tự động tải các images từ Docker Hub và khởi động toàn bộ hệ thống.

### Bước 3: Xác minh Trạng thái

Kiểm tra tất cả 8 services đã chạy thành công:

```bash
docker compose ps
