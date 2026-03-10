


Imports MySql.Data.MySqlClient

Public Class Form5
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call Koneksi()
        Dim cmd As New MySqlCommand("INSERT INTO user(nama, kelas, email, password) VALUES(@nama, @kelas, @em, @pw)", conn)

        cmd.Parameters.AddWithValue("@nama", TextBox1.Text)
        cmd.Parameters.AddWithValue("@kelas", TextBox2.Text)
        cmd.Parameters.AddWithValue("@em", TextBox3.Text)
        cmd.Parameters.AddWithValue("@pw", TextBox4.Text)

        Try
            cmd.ExecuteNonQuery()
            MessageBox.Show("Registrasi berhasil, silakan login!")
            Me.Hide()
            Form6.Show()
        Catch ex As Exception
            MessageBox.Show("Error saat registrasi: " & ex.Message)
        End Try

        conn.Close()


    End Sub

    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim Form6 As New Form6
        Form6.Show()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If (CheckBox1.Checked) Then
            TextBox4.UseSystemPasswordChar = False
        Else
            TextBox4.UseSystemPasswordChar = True

        End If
    End Sub
End Class