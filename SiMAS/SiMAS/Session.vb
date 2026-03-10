Module Session
    Public CurrentUserEmail As String
    Public CurrentUserNama As String
    Public CurrentUserKelas As String
    Public CurrentUserId As Integer
    Public CurrentUserRole As String = ""

    ' Tambahkan sub baru untuk menghapus session
    Public Sub ClearSession()
        CurrentUserEmail = ""
        CurrentUserNama = ""
        CurrentUserKelas = ""
        CurrentUserId = 0
        CurrentUserRole = ""
    End Sub
End Module
