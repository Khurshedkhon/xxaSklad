Imports MySql.Data.MySqlClient

''' <summary>
''' Сервис для работы с клиентами
''' </summary>
Public Class ClientService
    ''' <summary>
    ''' Получить всех клиентов
    ''' </summary>
    Public Shared Function GetAllClients() As List(Of Client)
        Try
            Dim query As String = "SELECT * FROM Clients WHERE IsActive = 1 ORDER BY ClientName"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query)
            Dim clients As New List(Of Client)

            For Each row As DataRow In dt.Rows
                clients.Add(MapRowToClient(row))
            Next

            Return clients
        Catch ex As Exception
            Throw New Exception("Ошибка при получении клиентов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить клиента по ID
    ''' </summary>
    Public Shared Function GetClientById(clientId As Integer) As Client
        Try
            Dim query As String = "SELECT * FROM Clients WHERE ClientId = @clientId"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@clientId", clientId))

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Return MapRowToClient(dt.Rows(0))
        Catch ex As Exception
            Throw New Exception("Ошибка при получении клиента: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Создать нового клиента
    ''' </summary>
    Public Shared Function CreateClient(clientName As String, phoneNumber As String, email As String, 
                                       address As String, city As String, postalCode As String, contactPerson As String) As Integer
        Try
            Dim query As String = "INSERT INTO Clients (ClientName, PhoneNumber, Email, Address, City, PostalCode, ContactPerson, IsActive) " &
                                 "VALUES (@clientName, @phoneNumber, @email, @address, @city, @postalCode, @contactPerson, 1)"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@clientName", clientName),
                New MySqlParameter("@phoneNumber", If(phoneNumber = "", DBNull.Value, phoneNumber)),
                New MySqlParameter("@email", If(email = "", DBNull.Value, email)),
                New MySqlParameter("@address", If(address = "", DBNull.Value, address)),
                New MySqlParameter("@city", If(city = "", DBNull.Value, city)),
                New MySqlParameter("@postalCode", If(postalCode = "", DBNull.Value, postalCode)),
                New MySqlParameter("@contactPerson", If(contactPerson = "", DBNull.Value, contactPerson))
            }

            DatabaseService.ExecuteCommand(query, parameters)
            
            Dim lastIdQuery As String = "SELECT LAST_INSERT_ID()"
            Dim lastId As Object = DatabaseService.ExecuteScalar(lastIdQuery)
            
            Return CInt(lastId)
        Catch ex As Exception
            Throw New Exception("Ошибка при создании клиента: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Обновить клиента
    ''' </summary>
    Public Shared Function UpdateClient(clientId As Integer, clientName As String, phoneNumber As String, email As String, 
                                       address As String, city As String, postalCode As String, contactPerson As String) As Boolean
        Try
            Dim query As String = "UPDATE Clients SET ClientName = @clientName, PhoneNumber = @phoneNumber, Email = @email, " &
                                 "Address = @address, City = @city, PostalCode = @postalCode, ContactPerson = @contactPerson, UpdatedDate = NOW() " &
                                 "WHERE ClientId = @clientId"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@clientName", clientName),
                New MySqlParameter("@phoneNumber", If(phoneNumber = "", DBNull.Value, phoneNumber)),
                New MySqlParameter("@email", If(email = "", DBNull.Value, email)),
                New MySqlParameter("@address", If(address = "", DBNull.Value, address)),
                New MySqlParameter("@city", If(city = "", DBNull.Value, city)),
                New MySqlParameter("@postalCode", If(postalCode = "", DBNull.Value, postalCode)),
                New MySqlParameter("@contactPerson", If(contactPerson = "", DBNull.Value, contactPerson)),
                New MySqlParameter("@clientId", clientId)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при обновлении клиента: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Удалить клиента (мягкое удаление)
    ''' </summary>
    Public Shared Function DeleteClient(clientId As Integer) As Boolean
        Try
            Dim query As String = "UPDATE Clients SET IsActive = 0 WHERE ClientId = @clientId"
            DatabaseService.ExecuteCommand(query, New MySqlParameter("@clientId", clientId))
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при удалении клиента: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Поиск клиентов по названию или телефону
    ''' </summary>
    Public Shared Function SearchClients(searchText As String) As List(Of Client)
        Try
            Dim query As String = "SELECT * FROM Clients WHERE IsActive = 1 AND (ClientName LIKE @search OR PhoneNumber LIKE @search OR Email LIKE @search) ORDER BY ClientName"
            Dim searchParam As String = "%" & searchText & "%"
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@search", searchParam))
            Dim clients As New List(Of Client)

            For Each row As DataRow In dt.Rows
                clients.Add(MapRowToClient(row))
            Next

            Return clients
        Catch ex As Exception
            Throw New Exception("Ошибка при поиске клиентов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Маппинг строки данных в объект Client
    ''' </summary>
    Private Shared Function MapRowToClient(row As DataRow) As Client
        Return New Client With {
            .ClientId = CInt(row("ClientId")),
            .ClientName = row("ClientName").ToString(),
            .PhoneNumber = If(IsDBNull(row("PhoneNumber")), "", row("PhoneNumber").ToString()),
            .Email = If(IsDBNull(row("Email")), "", row("Email").ToString()),
            .Address = If(IsDBNull(row("Address")), "", row("Address").ToString()),
            .City = If(IsDBNull(row("City")), "", row("City").ToString()),
            .PostalCode = If(IsDBNull(row("PostalCode")), "", row("PostalCode").ToString()),
            .ContactPerson = If(IsDBNull(row("ContactPerson")), "", row("ContactPerson").ToString()),
            .IsActive = CBool(row("IsActive"))
        }
    End Function
End Class
