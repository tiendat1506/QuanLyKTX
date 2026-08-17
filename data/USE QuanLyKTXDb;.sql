USE QuanLyKTXDb;
GO

DECLARE @j INT = 1;
DECLARE @hoList TABLE (ho NVARCHAR(20));
INSERT INTO @hoList VALUES (N'Nguyễn'),(N'Trần'),(N'Lê'),(N'Phạm'),(N'Hoàng'),(N'Huỳnh'),(N'Phan'),(N'Vũ'),(N'Võ'),(N'Đặng');

DECLARE @tenList TABLE (ten NVARCHAR(20));
INSERT INTO @tenList VALUES (N'An'),(N'Bình'),(N'Cường'),(N'Dũng'),(N'Hải'),(N'Hùng'),(N'Lan'),(N'Linh'),(N'Mai'),(N'Trang');

DECLARE @queList TABLE (que NVARCHAR(30));
INSERT INTO @queList VALUES (N'Hà Nội'),(N'Hải Phòng'),(N'Nam Định'),(N'Thanh Hóa'),(N'Nghệ An'),(N'Huế'),(N'Đà Nẵng'),(N'TP.HCM'),(N'Cần Thơ'),(N'An Giang');

WHILE @j <= 100
BEGIN
    DECLARE @ho NVARCHAR(20) = (SELECT TOP 1 ho FROM @hoList ORDER BY NEWID());
    DECLARE @ten NVARCHAR(20) = (SELECT TOP 1 ten FROM @tenList ORDER BY NEWID());
    DECLARE @que NVARCHAR(30) = (SELECT TOP 1 que FROM @queList ORDER BY NEWID());

    INSERT INTO NguoiThues (HoTen, CCCD, NgaySinh, GioiTinh, SDT, Email, QueQuan)
    VALUES (
        @ho + N' Văn ' + @ten,
        '070' + RIGHT('000000000' + CAST(2000000000 + @j AS VARCHAR), 9),
        DATEADD(YEAR, -20, DATEADD(DAY, -@j, GETDATE())),
        CASE WHEN @j % 2 = 0 THEN N'Nam' ELSE N'Nữ' END,
        '09' + RIGHT('00000000' + CAST(10000000 + @j AS VARCHAR), 8),
        'nguoithue' + CAST(@j AS VARCHAR) + '@gmail.com',
        @que
    );

    SET @j = @j + 1;
END