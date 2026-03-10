Imports MySql.Data.MySqlClient

Public Class Form11

    Dim totalPertemuan As Integer = 60

    Private Sub Form11_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call Koneksi()

        ' Label kelas dan guru
        Label5.Text = "Kelas: " & CurrentUserKelas

        ' Isi pilihan semester
        ComboBox1.Items.Clear()
        ComboBox1.Items.AddRange(New String() {"1", "2"})

        ' Awalnya tampilkan siswa berdasarkan guru login
        TampilSiswa()
    End Sub


    '=====================[ TAMPIL DATA SISWA SESUAI GURU LOGIN ]=====================
    Sub TampilSiswa()
        Try
            Koneksi()

            Dim sql As String = "SELECT nisn, nama FROM siswa WHERE id_guru=@idguru ORDER BY nama ASC"
            Cmd = New MySqlCommand(sql, conn)
            Cmd.Parameters.AddWithValue("@idguru", CurrentUserId)
            Rd = Cmd.ExecuteReader()

            DataGridView1.Rows.Clear()
            DataGridView1.Columns.Clear()
            DataGridView1.Columns.Add("NISN", "NISN")
            DataGridView1.Columns.Add("Nama", "Nama")

            While Rd.Read()
                DataGridView1.Rows.Add(Rd("nisn").ToString(), Rd("nama").ToString())
            End While

            Rd.Close()

            ' Format tampilan grid
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            DataGridView1.ReadOnly = True
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            DataGridView1.AllowUserToAddRows = False

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data siswa: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    '=====================[ TAMPIL REKAP ABSENSI PER SEMESTER (FILTER GURU) ]=====================
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.Text <> "" Then
            TampilRekap()
        End If
    End Sub

    Sub TampilRekap()
        Try
            Koneksi()

            ' Hapus isi dan kolom lama agar tidak dobel
            DataGridView1.DataSource = Nothing
            DataGridView1.Columns.Clear()

            Dim sql As String = "
            SELECT 
                s.nisn AS 'NISN',
                s.nama AS 'Nama',
                COALESCE(r.sakit, 0) AS 'Sakit',
                COALESCE(r.izin, 0) AS 'Izin',
                COALESCE(r.alpha, 0) AS 'Alpha',
                COALESCE(r.total_hadir, 0) AS 'Total Hadir'
            FROM siswa s
            LEFT JOIN rekap_absensi r 
                ON s.nisn = r.nisn AND r.semester = @semester
            WHERE s.id_guru = @idguru
            ORDER BY s.nama ASC"

            Da = New MySqlDataAdapter(sql, conn)
            Da.SelectCommand.Parameters.AddWithValue("@semester", ComboBox1.Text)
            Da.SelectCommand.Parameters.AddWithValue("@idguru", CurrentUserId)

            Ds = New DataSet()
            Da.Fill(Ds, "rekap")

            DataGridView1.DataSource = Ds.Tables("rekap")

            ' Format tampilan
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            DataGridView1.ReadOnly = True
            DataGridView1.AllowUserToAddRows = False
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        Catch ex As Exception
            MessageBox.Show("Gagal menampilkan rekap absensi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    '=====================[ TOMBOL INPUT ABSENSI (BUKA FORM12) ]=====================
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Pilih satu siswa terlebih dahulu!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If String.IsNullOrWhiteSpace(ComboBox1.Text) Then
                MessageBox.Show("Silakan pilih semester terlebih dahulu sebelum input absensi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim nisn As String = selectedRow.Cells("NISN").Value.ToString()
            Dim nama As String = selectedRow.Cells("Nama").Value.ToString()

            If String.IsNullOrEmpty(nisn) OrElse String.IsNullOrEmpty(nama) Then
                MessageBox.Show("Data siswa tidak valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim f As New Form12()
            f.NISN = nisn
            f.Nama = nama
            f.Semester = ComboBox1.Text
            f.ShowDialog()

            ' Refresh rekap setelah input
            TampilRekap()

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    '=====================[ TOMBOL UBAH ABSENSI ]=====================
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Try
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Pilih satu data siswa untuk diubah!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If String.IsNullOrWhiteSpace(ComboBox1.Text) Then
                MessageBox.Show("Pilih semester terlebih dahulu!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim nisn As String = DataGridView1.SelectedRows(0).Cells("NISN").Value.ToString()
            Dim nama As String = DataGridView1.SelectedRows(0).Cells("Nama").Value.ToString()

            ' Buka Form13, bukan Form12
            Dim f As New Form13()
            f.NISN = nisn
            f.Nama = nama
            f.Semester = ComboBox1.Text
            f.ShowDialog()

            ' Refresh tabel setelah ubah
            TampilRekap()

        Catch ex As Exception
            MessageBox.Show("Gagal membuka form ubah: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Form1.Show()
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form2.Show()
        Me.Close()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form3.Show()
        Me.Close()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show()
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form6.Show()
        Me.Close()
    End Sub
End Class
