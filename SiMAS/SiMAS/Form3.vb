Imports MySql.Data.MySqlClient

Public Class Form3
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs)

    End Sub
    Private Sub Form3_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Kalau user belum login, langsung arahkan ke Form6
        If String.IsNullOrEmpty(CurrentUserEmail) Then
            MessageBox.Show("Silakan login terlebih dahulu!", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Hide()
            Form6.Show()
            Exit Sub
        End If

        Try
            Call Koneksi()

            Dim sql As String = "SELECT * FROM user WHERE email=@Email"
            Dim cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Email", CurrentUserEmail)

            Dim dr As MySqlDataReader = cmd.ExecuteReader()
            If dr.Read() Then
                Label5.Text = dr("nama").ToString()
                Label6.Text = dr("email").ToString()
                Label8.Text = New String("*"c, dr("password").ToString().Length)
                Label8.Tag = dr("password").ToString()
            Else
                MessageBox.Show("Data user tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            dr.Close()

        Catch ex As Exception
            MessageBox.Show("Gagal mengambil data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub

    ' ======= UBAH DATA USER =======
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim newNama As String = InputBox("Masukkan nama baru:", "Ubah Nama", Label5.Text)
        Dim newEmail As String = InputBox("Masukkan email baru:", "Ubah Email", Label6.Text)

        If String.IsNullOrWhiteSpace(newNama) OrElse String.IsNullOrWhiteSpace(newEmail) Then
            MessageBox.Show("Nama dan email tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Call Koneksi()

            Dim sql As String = "UPDATE user SET nama=@nama, email=@newEmail WHERE email=@oldEmail"
            Dim cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@nama", newNama)
            cmd.Parameters.AddWithValue("@newEmail", newEmail)
            cmd.Parameters.AddWithValue("@oldEmail", CurrentUserEmail)

            Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

            If rowsAffected > 0 Then
                MessageBox.Show("Data berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Update tampilan label
                Label5.Text = newNama
                Label6.Text = newEmail

                ' Update session juga agar form lain ikut berubah
                CurrentUserNama = newNama
                CurrentUserEmail = newEmail
            Else
                MessageBox.Show("Gagal mengubah data user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal mengubah data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim newPassword As String = InputBox("Masukkan password baru:", "Ubah Password")

        If String.IsNullOrWhiteSpace(newPassword) Then
            MessageBox.Show("Password tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Call Koneksi()
            Dim sql As String = "UPDATE user SET password=@pass WHERE email=@Email"
            Dim cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@pass", newPassword)
            cmd.Parameters.AddWithValue("@Email", CurrentUserEmail)

            Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

            If rowsAffected > 0 Then
                MessageBox.Show("Password berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Label8.Text = New String("*"c, newPassword.Length)
                Label8.Tag = newPassword
            Else
                MessageBox.Show("Gagal mengubah password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal mengubah password: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try

    End Sub


    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Form15.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Form14.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        Form16.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        Form11.Show()
        Me.Hide()
    End Sub

    Private Sub Button9_Click_1(sender As Object, e As EventArgs) Handles Button9.Click
        If MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearSession()
            Form6.TextBox1.Text = ""
            Form6.TextBox2.Text = ""
            Form6.Show()
            Me.Hide()
        End If
    End Sub
End Class