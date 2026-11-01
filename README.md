# Lab 08 - Delegate & Event
Đỗ Văn Chung - 202418855 - Lớp 174373

## Chạy
Mở thư mục Source trong VS Code, mở Terminal:
```sh
dotnet run
```
Nếu terminal ở thư mục Lab08:
```sh
dotnet run --project Source/Lab08.csproj
```
Dự án net8.0. Nếu máy chỉ cài SDK/runtime .NET 10, đổi net8.0 thành net10.0
trong Lab08.csproj và lưu trước khi chạy. Không cần thư viện NuGet bên ngoài.

## Kết quả xác minh
Đã biên dịch với Roslyn từ SDK 8.0.415, chạy runtime8.0.21: 9 nhóm,14 đối chiếu,0 sai.
Môi trường hỗ trợ không chạy được CLI dotnet run do lỗi Process.GetStat;
đã gọi trực tiếp csc.dll với reference assemblies net8 và global using tương đương.
Log thật: Report/Ket_qua_chay.txt. Chưa chạy trên Windows của sinh viên.

## File nộp
Source chứa mã; Report chứa PDF + LaTeX; DESIGN.md ghi quy tắc trước cài đặt.
Khi đẩy GitHub, bỏ bin/obj. Bổ sung URL repo Lab08 vào báo cáo.
Ảnh chạy cá nhân đặt tại mục6. Không dùng ảnh Lab07 cho Lab08.

## Kiểm thử
9 nhóm minh họa các quy tắc trong đề; không phải số nhóm do đề bắt buộc.
Mỗi Check kiểm tra trạng thái/đếm sự kiện thật; nếu sai, exit code1.
Dữ liệu biên20/30/40,0,100 và ngoài miền -1/101 được sử dụng.
Mô hình cảnh báo theo vùng được giải thích trong DESIGN.md.

## Biên dịch báo cáo
Chạy XeLaTeX hai lần tại thư mục Report:
```sh
xelatex 202418855_DoVanChung_Lab08.tex
xelatex 202418855_DoVanChung_Lab08.tex
```
Cần font DejaVu và TikZ. File verification.tex phải ở cùng thư mục.
