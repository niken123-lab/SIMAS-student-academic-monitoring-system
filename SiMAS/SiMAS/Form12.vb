Imports MySql.Data.MySqlClient

Public Class Form12
    Public Property NISN As String
    Public Property Nama As String
    Public Property Semester As String
    Dim totalPertemuan As Integer = 60

    Private Sub Form12_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = NISN
        TextBox2.Text = Nama
        NumericUpDown2.Value = 0
        NumericUpDown3.Value = 0
        NumericUpDown4.Value = 0
        Me.Activate()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Koneksi()

            Dim izin As Integer = NumericUpDown2.Value
            Dim sakit As Integer = NumericUpDown3.Value
            Dim alpha As Integer = NumericUpDown4.Value
            Dim totalHadir As Integer = totalPertemuan - (izin + sakit + alpha)

            ' Query untuk insert atau update otomatis jika sudah ada NISN & Semester
            Dim sql As String = "
                INSERT INTO rekap_absensi (nisn, nama, semester, sakit, izin, alpha, total_hadir)
                VALUES (@nisn, @nama, @semester, @sakit, @izin, @alpha, @total)
                ON DUPLICATE KEY UPDATE
                    sakit=@sakit, izin=@izin, alpha=@alpha, total_hadir=@total, nama=@nama"

            Cmd = New MySqlCommand(sql, conn)
            Cmd.Parameters.AddWithValue("@nisn", NISN)
            Cmd.Parameters.AddWithValue("@nama", Nama)
            Cmd.Parameters.AddWithValue("@semester", Semester)
            Cmd.Parameters.AddWithValue("@sakit", sakit)
            Cmd.Parameters.AddWithValue("@izin", izin)
            Cmd.Parameters.AddWithValue("@alpha", alpha)
            Cmd.Parameters.AddWithValue("@total", totalHadir)
            Cmd.ExecuteNonQuery()

            MessageBox.Show("Data absensi berhasil disimpan untuk semester " & Semester, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Gagal menyimpan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
