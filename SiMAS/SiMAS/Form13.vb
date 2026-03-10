Imports MySql.Data.MySqlClient

Public Class Form13
    Public Property NISN As String
    Public Property Nama As String
    Public Property Semester As String
    Dim totalPertemuan As Integer = 60

    Private Sub Form13_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = NISN
        TextBox2.Text = Nama
        LoadDataAbsensi()
    End Sub

    Private Sub LoadDataAbsensi()
        Try
            Koneksi()

            Dim sql As String = "SELECT izin, sakit, alpha FROM rekap_absensi WHERE nisn=@nisn AND semester=@semester"
            Cmd = New MySqlCommand(sql, conn)
            Cmd.Parameters.AddWithValue("@nisn", NISN)
            Cmd.Parameters.AddWithValue("@semester", Semester)
            Rd = Cmd.ExecuteReader()

            If Rd.Read() Then
                NumericUpDown2.Value = Rd("izin")
                NumericUpDown3.Value = Rd("sakit")
                NumericUpDown4.Value = Rd("alpha")
            Else
                ' Kalau belum ada data, isi default 0
                NumericUpDown2.Value = 0
                NumericUpDown3.Value = 0
                NumericUpDown4.Value = 0
            End If

            Rd.Close()
        Catch ex As Exception
            MessageBox.Show("Gagal memuat data absensi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Koneksi()

            Dim izin As Integer = NumericUpDown2.Value
            Dim sakit As Integer = NumericUpDown3.Value
            Dim alpha As Integer = NumericUpDown4.Value
            Dim totalHadir As Integer = totalPertemuan - (izin + sakit + alpha)

            Dim sql As String = "
                UPDATE rekap_absensi 
                SET izin=@izin, sakit=@sakit, alpha=@alpha, total_hadir=@total 
                WHERE nisn=@nisn AND semester=@semester"

            Cmd = New MySqlCommand(sql, conn)
            Cmd.Parameters.AddWithValue("@izin", izin)
            Cmd.Parameters.AddWithValue("@sakit", sakit)
            Cmd.Parameters.AddWithValue("@alpha", alpha)
            Cmd.Parameters.AddWithValue("@total", totalHadir)
            Cmd.Parameters.AddWithValue("@nisn", NISN)
            Cmd.Parameters.AddWithValue("@semester", Semester)

            Dim result As Integer = Cmd.ExecuteNonQuery()

            If result > 0 Then
                MessageBox.Show("Data absensi berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Close()
            Else
                MessageBox.Show("Data tidak ditemukan untuk diperbarui.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal mengubah data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
