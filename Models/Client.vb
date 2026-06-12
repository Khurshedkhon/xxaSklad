''' <summary>
''' Модель клиента
''' </summary>
Public Class Client
    Public Property ClientId As Integer
    Public Property ClientName As String
    Public Property PhoneNumber As String
    Public Property Email As String
    Public Property Address As String
    Public Property City As String
    Public Property PostalCode As String
    Public Property ContactPerson As String
    Public Property IsActive As Boolean
    Public Property CreatedDate As DateTime
    Public Property UpdatedDate As DateTime

    Public Sub New()
    End Sub

    Public Sub New(clientId As Integer, clientName As String, phoneNumber As String, email As String)
        Me.ClientId = clientId
        Me.ClientName = clientName
        Me.PhoneNumber = phoneNumber
        Me.Email = email
    End Sub

    Public Overrides Function ToString() As String
        Return ClientName
    End Function
End Class
