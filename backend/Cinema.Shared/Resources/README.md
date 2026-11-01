# Messages.resx — message tập trung

Mọi message trả về client (lỗi nghiệp vụ, validation, lỗi hệ thống) nằm trong **`Messages.resx`**.
Code chỉ tham chiếu qua class strongly-typed `Messages` (file `Messages.Designer.cs`):

```csharp
throw new NotFoundException(Messages.NotFound_Room);
RuleFor(x => x.RoomId).GreaterThan(0).WithMessage(Messages.Validation_RoomId_Positive);
throw new BusinessRuleException(string.Format(Messages.Rule_CheckInNotOpenYet, minutes)); // message có {0}
```

## Quy ước đặt key: `<Nhóm>_<Mô tả tiếng Anh>`

| Nhóm | Dùng cho |
|---|---|
| `NotFound_` | `NotFoundException` (404) |
| `Conflict_` | `ConflictAppException` (409) |
| `Forbidden_` / `Unauthorized_` | 403 / 401 |
| `Invalid_` | `ValidationAppException` (400) |
| `Rule_` | `BusinessRuleException` (422) |
| `Validation_<Field>_<Rule>` | FluentValidation `.WithMessage(...)` |
| `Db_` / `System_` / `Common_` | middleware, lỗi DB, lỗi chung |
| `Success_` | message thành công của POST/PUT/DELETE (hiện thành toast ở frontend) |

## Message thành công

Gắn lên action bằng attribute (key kiểm tra lúc compile nhờ `nameof`):

```csharp
[HttpPost]
[SuccessMessage(nameof(Messages.Success_GenreCreated))]
public async Task<IActionResult> Create(...) => ...
```

`ApiResponseFilter` đọc attribute và điền `ApiResponse.Message`. Message phụ thuộc kết quả thì trả trực tiếp
`Ok(ApiResponse.SuccessResponse(data, Messages.Success_X))`. Endpoint không cần toast (bulk, upload, login...)
thì không gắn gì — `message = null` và frontend sẽ bỏ qua.

## Thêm / sửa message

1. Thêm `<data name="..."><value>...</value></data>` vào `Messages.resx`
   (mở bằng Visual Studio / Rider để dùng editor dạng bảng, Designer tự sinh lại).
2. Nếu sửa bằng tay hoặc CLI, thêm property tương ứng vào `Messages.Designer.cs`.
   Test `MessagesResourceTests` sẽ fail nếu hai file lệch nhau.
3. Message có tham số dùng `{0}`, `{1}` và gọi `string.Format(Messages.Key, ...)`.
4. Đổi tên key: dùng Rename (F2) của IDE để cập nhật mọi chỗ dùng.

## Thêm ngôn ngữ

Ngôn ngữ gốc là tiếng Việt (`NeutralLanguage=vi`). Thêm `Messages.en.resx` cạnh file gốc,
chỉ cần các key cần dịch; key thiếu sẽ tự rơi về tiếng Việt. Chọn ngôn ngữ theo request bằng
`app.UseRequestLocalization(...)` rồi đọc `Messages.X` như bình thường (ResourceManager dùng
`CurrentUICulture`).
