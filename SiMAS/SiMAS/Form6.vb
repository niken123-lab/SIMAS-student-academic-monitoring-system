Imports MySql.Data.MySqlClient

Public Class Form6

    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox2.UseSystemPasswordChar = True

        ' Bersihkan input setiap kali form login dibuka
        TextBox1.Clear()
        TextBox2.Clear()

        ' Hapus session sebelumnya
        ClearSession()
        CenterPanel()
    End Sub

    Private Sub Form6_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        CenterPanel()
    End Sub

    Private Sub CenterPanel()
        If Panel1 IsNot Nothing AndAlso PictureBox1 IsNot Nothing Then
            Dim spacing As Integer = 20
            Dim totalWidth As Integer = PictureBox1.Width + spacing + Panel1.Width
            Dim startX As Integer = (Me.ClientSize.Width - totalWidth) \ 2

            PictureBox1.Left = startX
            Panel1.Left = PictureBox1.Right + spacing

            Dim centerY As Integer = (Me.ClientSize.Height - Math.Max(PictureBox1.Height, Panel1.Height)) \ 2
            PictureBox1.Top = centerY
            Panel1.Top = centerY
        ElseIf Panel1 IsNot Nothing Then
            Panel1.Left = (Me.ClientSize.Width - Panel1.Width) \ 2
            Panel1.Top = (Me.ClientSize.Height - Panel1.Height) \ 2
        End If
    End Sub

    ' ================== LOGIN ==================
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Validasi input
        If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MessageBox.Show("Email dan Password harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Call Koneksi()
            Dim isLoginSuccess As Boolean = False

            ' ================== CEK LOGIN ADMIN ==================
            Dim sqlAdmin As String = "SELECT * FROM admin WHERE email=@em AND password=@pw"
            Using cmdAdmin As New MySqlCommand(sqlAdmin, conn)
                cmdAdmin.Parameters.AddWithValue("@em", TextBox1.Text)
                cmdAdmin.Parameters.AddWithValue("@pw", TextBox2.Text)

                Using rdAdmin As MySqlDataReader = cmdAdmin.ExecuteReader()
                    If rdAdmin.Read() Then
                        isLoginSuccess = True

                        ' ✅ Simpan data admin ke variabel global
                        CurrentUserEmail = TextBox1.Text
                        CurrentUserNama = "Admin SIMAS"
                        CurrentUserRole = "admin"
                        CurrentUserId = 999
                        rdAdmin.Close()

                        MessageBox.Show("Login sebagai ADMIN berhasil!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' 🔹 Buka Form1 (Admin)
                        Form1.Show()
                        Me.Hide()
                        Return
                    End If
                End Using
            End Using

            ' ================== CEK LOGIN USER BIASA ==================
            If Not isLoginSuccess Then
                Dim sqlUser As String = "SELECT id_guru, nama, kelas, email FROM user WHERE email=@em AND password=@pw"
                Using cmdUser As New MySqlCommand(sqlUser, conn)
                    cmdUser.Parameters.AddWithValue("@em", TextBox1.Text)
                    cmdUser.Parameters.AddWithValue("@pw", TextBox2.Text)

                    Using rdUser As MySqlDataReader = cmdUser.ExecuteReader()
                        If rdUser.Read() Then
                            isLoginSuccess = True

                            ' ✅ Simpan data user ke variabel global
                            CurrentUserEmail = rdUser("email").ToString()
                            CurrentUserNama = rdUser("nama").ToString()
                            CurrentUserKelas = rdUser("kelas").ToString()
                            CurrentUserId = Convert.ToInt32(rdUser("id_guru"))
                            CurrentUserRole = "user"

                            ' 🔹 Debug tambahan untuk memastikan variabel benar
                            MessageBox.Show(
                                "Login Wali Kelas Berhasil!" & vbCrLf &
                                "Nama: " & CurrentUserNama & vbCrLf &
                                "Kelas: " & CurrentUserKelas & vbCrLf &
                                "ID Guru: " & CurrentUserId
                            )

                            rdUser.Close()

                            ' 🔹 Buka Form15 (Guru)
                            Form15.Show()
                            Form15.tampilData()
                            Me.Hide()
                        Else
                            MessageBox.Show("Email atau Password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End Using
                End Using
            End If

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat login: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub

    ' ================== SHOW / HIDE PASSWORD ==================
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        TextBox2.UseSystemPasswordChar = Not CheckBox1.Checked
    End Sub

    ' ================== BUKA FORM REGISTER ==================
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim form5 As New Form5
        form5.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim form5 As New Form5
        form5.Show()
    End Sub

End Class
