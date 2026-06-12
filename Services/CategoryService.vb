Imports MySql.Data.MySqlClient

''' <summary>
''' Сервис для работы с категориями товаров
''' </summary>
Public Class CategoryService
    ''' <summary>
    ''' Получить все категории
    ''' </summary>
    Public Shared Function GetAllCategories() As List(Of Category)
        Try
            Dim query As String = "SELECT * FROM Categories WHERE IsActive = 1 ORDER BY CategoryName"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query)
            Dim categories As New List(Of Category)

            For Each row As DataRow In dt.Rows
                categories.Add(New Category With {
                    .CategoryId = CInt(row("CategoryId")),
                    .CategoryName = row("CategoryName").ToString(),
                    .Description = If(IsDBNull(row("Description")), "", row("Description").ToString()),
                    .IsActive = CBool(row("IsActive"))
                })
            Next

            Return categories
        Catch ex As Exception
            Throw New Exception("Ошибка при получении категорий: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить категорию по ID
    ''' </summary>
    Public Shared Function GetCategoryById(categoryId As Integer) As Category
        Try
            Dim query As String = "SELECT * FROM Categories WHERE CategoryId = @categoryId"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@categoryId", categoryId))

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim row As DataRow = dt.Rows(0)
            Return New Category With {
                .CategoryId = CInt(row("CategoryId")),
                .CategoryName = row("CategoryName").ToString(),
                .Description = If(IsDBNull(row("Description")), "", row("Description").ToString()),
                .IsActive = CBool(row("IsActive"))
            }
        Catch ex As Exception
            Throw New Exception("Ошибка при получении категории: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Создать новую категорию
    ''' </summary>
    Public Shared Function CreateCategory(categoryName As String, description As String) As Integer
        Try
            Dim query As String = "INSERT INTO Categories (CategoryName, Description, IsActive) VALUES (@categoryName, @description, 1)"
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@categoryName", categoryName),
                New MySqlParameter("@description", description)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            
            Dim lastIdQuery As String = "SELECT LAST_INSERT_ID()"
            Dim lastId As Object = DatabaseService.ExecuteScalar(lastIdQuery)
            
            Return CInt(lastId)
        Catch ex As Exception
            Throw New Exception("Ошибка при создании категории: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Обновить категорию
    ''' </summary>
    Public Shared Function UpdateCategory(categoryId As Integer, categoryName As String, description As String) As Boolean
        Try
            Dim query As String = "UPDATE Categories SET CategoryName = @categoryName, Description = @description WHERE CategoryId = @categoryId"
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@categoryName", categoryName),
                New MySqlParameter("@description", description),
                New MySqlParameter("@categoryId", categoryId)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при обновлении категории: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Удалить категорию (мягкое удаление)
    ''' </summary>
    Public Shared Function DeleteCategory(categoryId As Integer) As Boolean
        Try
            Dim query As String = "UPDATE Categories SET IsActive = 0 WHERE CategoryId = @categoryId"
            DatabaseService.ExecuteCommand(query, New MySqlParameter("@categoryId", categoryId))
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при удалении категории: " & ex.Message)
        End Try
    End Function
End Class
