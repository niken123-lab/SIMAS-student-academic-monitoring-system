Imports MySql.Data.MySqlClient

Public Class Form6

    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox2.UseSystemPasswordChar = True

        ' Bersihkan input setiap kali form login dibuka
        TextBox1.Clear()
        TextBox2.Clear()

        ' Hapus session sebelumnya (jaga-jaga kalau user belum logout dengan benar)
        ClearSession()
    End Sub

    ' ================== LOGIN ==================
    Private Sub ButtonLogin_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Validasi input
        If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MessageBox.Show("Email dan Password harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Call Koneksi()

            ' ✅ Gunakan query eksplisit, hindari SELECT * untuk keamanan
            Dim sql As String = "SELECT id_guru, nama, kelas, email 
                                 FROM user 
                                 WHERE email=@em AND password=@pw"

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@em", TextBox1.Text)
                cmd.Parameters.AddWithValue("@pw", TextBox2.Text)

                Dim rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' 🔹 Simpan data user ke Session
                    CurrentUserEmail = rd("email").ToString()
                    CurrentUserNama = rd("nama").ToString()
                    CurrentUserKelas = rd("kelas").ToString()
                    CurrentUserId = Convert.ToInt32(rd("id_guru"))

                    rd.Close()

                    MessageBox.Show("Login berhasil!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' 🔹 Buka Form1 dan tampilkan data
                    Form1.Show()
                    Form1.tampilData()

                    ' 🔹 Sembunyikan form login
                    Me.Hide()
                Else
                    MessageBox.Show("Email atau Password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using

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
