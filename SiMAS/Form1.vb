Imports MySql.Data.MySqlClient

Public Class Form1
    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Pastikan hanya tampil data kalau sudah login
        If CurrentUserId <> 0 Then
            tampilData()
        Else
            ' Kalau belum login, kembali ke form login
            MessageBox.Show("Silakan login terlebih dahulu!", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Hide()
            Form6.Show()
        End If
    End Sub
    Public Sub tampilData()
        Label12.Text = CurrentUserNama
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            ' --- tampilkan siswa hanya milik guru login ---
            Dim sqlSiswa As String =
                 "SELECT " &
                 "`NISN` AS 'NISN', " &
                 "`NAMA` AS 'NAMA', " &
                 "`NOMOR HP` AS 'NOMOR HP', " &
                 "`NAMA WALI` AS 'NAMA WALI', " &
                 "`NOMOR WALI` AS 'NOMOR WALI', " &
                 "`ALAMAT` AS 'ALAMAT' " &
                 "FROM siswa WHERE id_guru=@id_guru"

            Dim da As New MySqlDataAdapter(sqlSiswa, conn)
            da.SelectCommand.Parameters.AddWithValue("@id_guru", CurrentUserId)

            Dim ds As New DataSet()
            da.Fill(ds, "siswa")
            DataGridView1.DataSource = ds.Tables("siswa")

            ' --- jumlah siswa ---
            Dim jumlah As Integer = ds.Tables("siswa").Rows.Count
            Label6.Text = jumlah.ToString()

            ' --- rata-rata nilai semua siswa milik guru login ---
            Dim sqlRata As String = "SELECT AVG(`RATA RATA`) FROM nilai WHERE id_guru=@id_guru"
            Dim cmdRata As New MySqlCommand(sqlRata, conn)
            cmdRata.Parameters.AddWithValue("@id_guru", CurrentUserId)

            Dim rataObj = cmdRata.ExecuteScalar()

            If IsDBNull(rataObj) OrElse rataObj Is Nothing Then
                Label8.Text = "0"
            Else
                Dim rata As Decimal = Convert.ToDecimal(rataObj)
                Label8.Text = rata.ToString("0.00")
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal mengambil data: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(230, 230, 250)

        Timer1.Interval = 1000
        Timer1.Start()
        tampilData()
    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form2.RefreshData()
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form3.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form11.Show()
        Me.Hide()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim frm As New Form7()
        frm.Show()
    End Sub

    ' === UBAH SISWA ===
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih row terlebih dahulu")
            Exit Sub
        End If

        Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)

        ' Pastikan siswa yang dipilih milik guru yang login
        Dim idGuruRow As Integer = Convert.ToInt32(row.Cells("id_guru").Value)
        If idGuruRow <> CurrentUserId Then
            MessageBox.Show("Anda tidak berhak mengubah data siswa milik guru lain!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Panggil Form8 untuk edit
        Dim formubah As New Form8(
            row.Cells("NISN").Value.ToString(),
            row.Cells("NAMA").Value.ToString(),
            row.Cells("NOMOR HP").Value.ToString(),
            row.Cells("NAMA WALI").Value.ToString(),
            row.Cells("NOMOR WALI").Value.ToString(),
            row.Cells("ALAMAT").Value.ToString())

        ' Gunakan ShowDialog agar edit selesai dulu sebelum lanjut
        formubah.ShowDialog()

        ' Refresh data setelah edit
        tampilData()
    End Sub

    ' === HAPUS SISWA + NILAINYA ===
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih row yang mau dihapus!")
            Exit Sub
        End If

        Dim nisn As String = DataGridView1.SelectedRows(0).Cells("nisn").Value.ToString()

        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            ' Hapus nilai dulu (hanya milik guru login)
            Dim sqlNilai As String = "DELETE FROM `nilai` WHERE `nisn`=@nisn AND `id_guru`=@id_guru"
            Using cmdNilai As New MySqlCommand(sqlNilai, conn)
                cmdNilai.Parameters.AddWithValue("@nisn", nisn)
                cmdNilai.Parameters.AddWithValue("@id_guru", CurrentUserId)
                cmdNilai.ExecuteNonQuery()
            End Using

            ' Hapus siswa (hanya milik guru login)
            Dim sqlSiswa As String = "DELETE FROM `siswa` WHERE `nisn`=@nisn AND `id_guru`=@id_guru"
            Using cmdSiswa As New MySqlCommand(sqlSiswa, conn)
                cmdSiswa.Parameters.AddWithValue("@nisn", nisn)
                cmdSiswa.Parameters.AddWithValue("@id_guru", CurrentUserId)
                cmdSiswa.ExecuteNonQuery()
            End Using

            MessageBox.Show("Data siswa dan nilainya berhasil dihapus!")
            tampilData()

        Catch ex As Exception
            MessageBox.Show("Error hapus: " & ex.Message)
        Finally
            conn.Close()
        End Try

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ToolStripStatusLabel1.Text = "Date/Time: " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click

        If MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearSession()
            Form6.TextBox1.Text = ""
            Form6.TextBox2.Text = ""
            Form6.Show()
            Me.Hide()
        End If
    End Sub


End Class