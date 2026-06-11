Imports MySql.Data.MySqlClient

''' <summary>
''' Сервис для работы с базой данных MySQL
''' </summary>
Public Class DatabaseService
    Private Shared ReadOnly ConnectionString As String = ConfigurationManager.ConnectionStrings("MySQLConnection").ConnectionString

    ''' <summary>
    ''' Получить подключение к БД
    ''' </summary>
    Public Shared Function GetConnection() As MySqlConnection
        Return New MySqlConnection(ConnectionString)
    End Function

    ''' <summary>
    ''' Проверить подключение к БД
    ''' </summary>
    Public Shared Function TestConnection() As Boolean
        Try
            Using connection As MySqlConnection = GetConnection()
                connection.Open()
                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Ошибка подключения к БД: " & ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Выполнить SQL команду
    ''' </summary>
    Public Shared Function ExecuteCommand(query As String, ParamArray parameters As MySqlParameter()) As Integer
        Try
            Using connection As MySqlConnection = GetConnection()
                Using command As New MySqlCommand(query, connection)
                    For Each param In parameters
                        command.Parameters.Add(param)
                    Next
                    connection.Open()
                    Return command.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Ошибка при выполнении команды: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Выполнить SQL запрос и получить результат
    ''' </summary>
    Public Shared Function ExecuteQuery(query As String, ParamArray parameters As MySqlParameter()) As DataTable
        Try
            Dim dt As New DataTable()
            Using connection As MySqlConnection = GetConnection()
                Using command As New MySqlCommand(query, connection)
                    For Each param In parameters
                        command.Parameters.Add(param)
                    Next
                    Using adapter As New MySqlDataAdapter(command)
                        adapter.Fill(dt)
                    End Using
                End Using
            End Using
            Return dt
        Catch ex As Exception
            Throw New Exception("Ошибка при выполнении запроса: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Выполнить SQL запрос и получить одно значение
    ''' </summary>
    Public Shared Function ExecuteScalar(query As String, ParamArray parameters As MySqlParameter()) As Object
        Try
            Using connection As MySqlConnection = GetConnection()
                Using command As New MySqlCommand(query, connection)
                    For Each param In parameters
                        command.Parameters.Add(param)
                    Next
                    connection.Open()
                    Return command.ExecuteScalar()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Ошибка при выполнении запроса: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Начать транзакцию
    ''' </summary>
    Public Shared Function BeginTransaction(connection As MySqlConnection) As MySqlTransaction
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        Return connection.BeginTransaction()
    End Function
End Class
