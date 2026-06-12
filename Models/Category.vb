''' <summary>
''' Модель категории товаров
''' </summary>
Public Class Category
    Public Property CategoryId As Integer
    Public Property CategoryName As String
    Public Property Description As String
    Public Property IsActive As Boolean
    Public Property CreatedDate As DateTime

    Public Sub New()
    End Sub

    Public Sub New(categoryId As Integer, categoryName As String, description As String)
        Me.CategoryId = categoryId
        Me.CategoryName = categoryName
        Me.Description = description
    End Sub

    Public Overrides Function ToString() As String
        Return CategoryName
    End Function
End Class
