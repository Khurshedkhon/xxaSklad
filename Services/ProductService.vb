Imports MySql.Data.MySqlClient

''' <summary>
''' Сервис для работы с товарами
''' </summary>
Public Class ProductService
    ''' <summary>
    ''' Получить все товары
    ''' </summary>
    Public Shared Function GetAllProducts() As List(Of Product)
        Try
            Dim query As String = "SELECT p.*, c.CategoryName, COALESCE(s.Quantity, 0) as CurrentStock FROM Products p " &
                                 "LEFT JOIN Categories c ON p.CategoryId = c.CategoryId " &
                                 "LEFT JOIN Stock s ON p.ProductId = s.ProductId " &
                                 "WHERE p.IsActive = 1 ORDER BY p.ProductName"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query)
            Dim products As New List(Of Product)

            For Each row As DataRow In dt.Rows
                products.Add(MapRowToProduct(row))
            Next

            Return products
        Catch ex As Exception
            Throw New Exception("Ошибка при получении товаров: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить товар по ID
    ''' </summary>
    Public Shared Function GetProductById(productId As Integer) As Product
        Try
            Dim query As String = "SELECT p.*, c.CategoryName, COALESCE(s.Quantity, 0) as CurrentStock FROM Products p " &
                                 "LEFT JOIN Categories c ON p.CategoryId = c.CategoryId " &
                                 "LEFT JOIN Stock s ON p.ProductId = s.ProductId " &
                                 "WHERE p.ProductId = @productId"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@productId", productId))

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Return MapRowToProduct(dt.Rows(0))
        Catch ex As Exception
            Throw New Exception("Ошибка при получении товара: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить товары по категории
    ''' </summary>
    Public Shared Function GetProductsByCategory(categoryId As Integer) As List(Of Product)
        Try
            Dim query As String = "SELECT p.*, c.CategoryName, COALESCE(s.Quantity, 0) as CurrentStock FROM Products p " &
                                 "LEFT JOIN Categories c ON p.CategoryId = c.CategoryId " &
                                 "LEFT JOIN Stock s ON p.ProductId = s.ProductId " &
                                 "WHERE p.CategoryId = @categoryId AND p.IsActive = 1 ORDER BY p.ProductName"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@categoryId", categoryId))
            Dim products As New List(Of Product)

            For Each row As DataRow In dt.Rows
                products.Add(MapRowToProduct(row))
            Next

            Return products
        Catch ex As Exception
            Throw New Exception("Ошибка при получении товаров категории: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Создать новый товар
    ''' </summary>
    Public Shared Function CreateProduct(productName As String, productCode As String, categoryId As Integer, 
                                        description As String, unitOfMeasure As String, costPrice As Decimal, 
                                        sellingPrice As Decimal) As Integer
        Try
            Dim query As String = "INSERT INTO Products (ProductName, ProductCode, CategoryId, Description, UnitOfMeasure, CostPrice, SellingPrice, IsActive) " &
                                 "VALUES (@productName, @productCode, @categoryId, @description, @unitOfMeasure, @costPrice, @sellingPrice, 1)"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@productName", productName),
                New MySqlParameter("@productCode", productCode),
                New MySqlParameter("@categoryId", categoryId),
                New MySqlParameter("@description", description),
                New MySqlParameter("@unitOfMeasure", unitOfMeasure),
                New MySqlParameter("@costPrice", costPrice),
                New MySqlParameter("@sellingPrice", sellingPrice)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            
            ' Получить ID вставленного товара
            Dim lastIdQuery As String = "SELECT LAST_INSERT_ID()"
            Dim lastId As Object = DatabaseService.ExecuteScalar(lastIdQuery)
            
            ' Создать запись в таблице остатков
            Dim stockQuery As String = "INSERT INTO Stock (ProductId, Quantity, MinimumLevel, MaximumLevel) VALUES (@productId, 0, 10, 1000)"
            DatabaseService.ExecuteCommand(stockQuery, New MySqlParameter("@productId", lastId))
            
            Return CInt(lastId)
        Catch ex As Exception
            Throw New Exception("Ошибка при создании товара: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Обновить товар
    ''' </summary>
    Public Shared Function UpdateProduct(productId As Integer, productName As String, productCode As String, 
                                        categoryId As Integer, description As String, unitOfMeasure As String, 
                                        costPrice As Decimal, sellingPrice As Decimal) As Boolean
        Try
            Dim query As String = "UPDATE Products SET ProductName = @productName, ProductCode = @productCode, " &
                                 "CategoryId = @categoryId, Description = @description, UnitOfMeasure = @unitOfMeasure, " &
                                 "CostPrice = @costPrice, SellingPrice = @sellingPrice, UpdatedDate = NOW() " &
                                 "WHERE ProductId = @productId"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@productName", productName),
                New MySqlParameter("@productCode", productCode),
                New MySqlParameter("@categoryId", categoryId),
                New MySqlParameter("@description", description),
                New MySqlParameter("@unitOfMeasure", unitOfMeasure),
                New MySqlParameter("@costPrice", costPrice),
                New MySqlParameter("@sellingPrice", sellingPrice),
                New MySqlParameter("@productId", productId)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при обновлении товара: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Удалить товар (мягкое удаление)
    ''' </summary>
    Public Shared Function DeleteProduct(productId As Integer) As Boolean
        Try
            Dim query As String = "UPDATE Products SET IsActive = 0 WHERE ProductId = @productId"
            DatabaseService.ExecuteCommand(query, New MySqlParameter("@productId", productId))
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при удалении товара: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Поиск товаров по названию или коду
    ''' </summary>
    Public Shared Function SearchProducts(searchText As String) As List(Of Product)
        Try
            Dim query As String = "SELECT p.*, c.CategoryName, COALESCE(s.Quantity, 0) as CurrentStock FROM Products p " &
                                 "LEFT JOIN Categories c ON p.CategoryId = c.CategoryId " &
                                 "LEFT JOIN Stock s ON p.ProductId = s.ProductId " &
                                 "WHERE p.IsActive = 1 AND (p.ProductName LIKE @search OR p.ProductCode LIKE @search) ORDER BY p.ProductName"
            
            Dim searchParam As String = "%" & searchText & "%"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@search", searchParam))
            Dim products As New List(Of Product)

            For Each row As DataRow In dt.Rows
                products.Add(MapRowToProduct(row))
            Next

            Return products
        Catch ex As Exception
            Throw New Exception("Ошибка при поиске товаров: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Маппинг строки данных в объект Product
    ''' </summary>
    Private Shared Function MapRowToProduct(row As DataRow) As Product
        Return New Product With {
            .ProductId = CInt(row("ProductId")),
            .ProductName = row("ProductName").ToString(),
            .ProductCode = row("ProductCode").ToString(),
            .CategoryId = CInt(row("CategoryId")),
            .CategoryName = row("CategoryName").ToString(),
            .Description = If(IsDBNull(row("Description")), "", row("Description").ToString()),
            .UnitOfMeasure = row("UnitOfMeasure").ToString(),
            .CostPrice = CDec(row("CostPrice")),
            .SellingPrice = CDec(row("SellingPrice")),
            .IsActive = CBool(row("IsActive")),
            .CurrentStock = CInt(row("CurrentStock"))
        }
    End Function
End Class
