Module Session
    Public CurrentUserEmail As String
    Public CurrentUserNama As String
    Public CurrentUserKelas As String
    Public CurrentUserId As Integer

    ' Tambahkan sub baru untuk menghapus session
    Public Sub ClearSession()
        CurrentUserEmail = ""
        CurrentUserNama = ""
        CurrentUserKelas = ""
        CurrentUserId = 0
    End Sub
End Module
