Imports MySql.Data.MySqlClient

''' <summary>
''' Сервис для работы с пользователями (авторизация)
''' </summary>
Public Class UserService
    ''' <summary>
    ''' Проверить учетные данные пользователя
    ''' </summary>
    Public Shared Function AuthenticateUser(username As String, password As String) As User
        Try
            Dim query As String = "SELECT * FROM Users WHERE Username = @username AND IsActive = 1 LIMIT 1"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@username", username))

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim row As DataRow = dt.Rows(0)
            Dim storedHash As String = row("PasswordHash").ToString()
            
            ' Проверить пароль (используется простой хеш SHA256)
            If VerifyPassword(password, storedHash) Then
                Dim user As New User With {
                    .UserId = CInt(row("UserId")),
                    .Username = row("Username").ToString(),
                    .FullName = row("FullName").ToString(),
                    .Email = row("Email").ToString(),
                    .IsActive = CBool(row("IsActive"))
                }
                
                ' Обновить время последнего входа
                UpdateLastLoginDate(user.UserId)
                Return user
            End If

            Return Nothing
        Catch ex As Exception
            Throw New Exception("Ошибка при проверке учетных данных: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить пользователя по ID
    ''' </summary>
    Public Shared Function GetUserById(userId As Integer) As User
        Try
            Dim query As String = "SELECT * FROM Users WHERE UserId = @userId"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@userId", userId))

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim row As DataRow = dt.Rows(0)
            Return New User With {
                .UserId = CInt(row("UserId")),
                .Username = row("Username").ToString(),
                .FullName = row("FullName").ToString(),
                .Email = row("Email").ToString(),
                .IsActive = CBool(row("IsActive"))
            }
        Catch ex As Exception
            Throw New Exception("Ошибка при получении пользователя: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить всех активных пользователей
    ''' </summary>
    Public Shared Function GetAllUsers() As List(Of User)
        Try
            Dim query As String = "SELECT * FROM Users WHERE IsActive = 1 ORDER BY FullName"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query)
            Dim users As New List(Of User)

            For Each row As DataRow In dt.Rows
                users.Add(New User With {
                    .UserId = CInt(row("UserId")),
                    .Username = row("Username").ToString(),
                    .FullName = row("FullName").ToString(),
                    .Email = row("Email").ToString(),
                    .IsActive = CBool(row("IsActive"))
                })
            Next

            Return users
        Catch ex As Exception
            Throw New Exception("Ошибка при получении списка пользователей: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Создать нового пользователя
    ''' </summary>
    Public Shared Function CreateUser(username As String, password As String, fullName As String, email As String) As Boolean
        Try
            Dim passwordHash As String = HashPassword(password)
            Dim query As String = "INSERT INTO Users (Username, PasswordHash, FullName, Email, IsActive) VALUES (@username, @password, @fullName, @email, 1)"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@username", username),
                New MySqlParameter("@password", passwordHash),
                New MySqlParameter("@fullName", fullName),
                New MySqlParameter("@email", email)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при создании пользователя: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Обновить пароль пользователя
    ''' </summary>
    Public Shared Function UpdatePassword(userId As Integer, newPassword As String) As Boolean
        Try
            Dim passwordHash As String = HashPassword(newPassword)
            Dim query As String = "UPDATE Users SET PasswordHash = @password WHERE UserId = @userId"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@password", passwordHash),
                New MySqlParameter("@userId", userId)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при обновлении пароля: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Обновить дату последнего входа
    ''' </summary>
    Private Shared Sub UpdateLastLoginDate(userId As Integer)
        Try
            Dim query As String = "UPDATE Users SET LastLoginDate = NOW() WHERE UserId = @userId"
            DatabaseService.ExecuteCommand(query, New MySqlParameter("@userId", userId))
        Catch ex As Exception
            ' Игнорируем ошибку обновления времени входа
        End Try
    End Sub

    ''' <summary>
    ''' Хеширование пароля (SHA256)
    ''' </summary>
    Private Shared Function HashPassword(password As String) As String
        Using sha256 = System.Security.Cryptography.SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
            Return Convert.ToBase64String(bytes)
        End Using
    End Function

    ''' <summary>
    ''' Проверить пароль
    ''' </summary>
    Private Shared Function VerifyPassword(password As String, hash As String) As Boolean
        Dim hashOfInput As String = HashPassword(password)
        Return hashOfInput.Equals(hash)
    End Function
End Class
