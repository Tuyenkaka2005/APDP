## HƯỚNG DẪN CHẠY LẦN ĐẦU (CHO MỌI THÀNH VIÊN)

1. Pull code từ GitHub
2. Mở SSMS → kết nối (localdb)\MSSQLLocalDB
3. Mở file Database/Setup.sql → nhấn F5 (chạy hết) (Chạy trong SSMS)
4. Mở terminal tại thư mục project → chạy:
   ```bash
   dotnet ef database update
