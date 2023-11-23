*. Được xây dựng trên Jetbrains Rider với phiên bản .NET 8.0
========================================================================================================================


--- Hướng dẫn sinh Database và cấu hình các Model ---

1. Mở thư mục "MVC" trên CMD hoặc Terminal

2. "dotnet ef migrations add mfr_migration" => sinh Migrations

3. "dotnet ef database update" => sinh Database dựa trên Migrations

4. "dotnet ef dbcontext optimize -o ../DataBase/Context/Optimize" => tối ưu các Model

!. Chạy lại bước 4 khi Model hoặc DbContext thay đổi
========================================================================================================================


--- Chú thích ---
(*.) : Ghi chú
(!.) : Cảnh báo/Lưu ý
(<./>) : Tham số
(=>) : Mục đích, kết quả đạt được,...
========================================================================================================================