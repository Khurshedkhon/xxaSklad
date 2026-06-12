''' <summary>
''' Модель пользователя системы
''' </summary>
Public Class User
    Public Property UserId As Integer
    Public Property Username As String
    Public Property PasswordHash As String
    Public Property FullName As String
    Public Property Email As String
    Public Property IsActive As Boolean
    Public Property CreatedDate As DateTime
    Public Property LastLoginDate As DateTime

    Public Sub New()
    End Sub

    Public Sub New(userId As Integer, username As String, fullName As String, email As String, isActive As Boolean)
        Me.UserId = userId
        Me.Username = username
        Me.FullName = fullName
        Me.Email = email
        Me.IsActive = isActive
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0} ({1})", FullName, Username)
    End Function
End Class
