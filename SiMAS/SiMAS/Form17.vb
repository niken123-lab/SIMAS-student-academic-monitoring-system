Imports MySql.Data.MySqlClient

Public Class Form17
    Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
    Dim dtAbsensi As DataTable
    Dim dvAbsensi As DataView

    ' =================== LOAD FORM ===================
    Private Sub Form17_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' === Isi ComboBox Kelas ===
            ComboBox1.Items.Clear()
            conn.Open()
            Dim cmdKelas As New MySqlCommand("SELECT DISTINCT kelas FROM user ORDER BY kelas", conn)
            Dim rd = cmdKelas.ExecuteReader()
            While rd.Read()
                ComboBox1.Items.Add(rd("kelas").ToString())
            End While
            rd.Close()
            conn.Close()

            ' === Isi ComboBox Semester ===
            ComboBox2.Items.Clear()
            ComboBox2.Items.AddRange(New String() {"1", "2"})

            ' === Tampilkan semua siswa dulu ===
            TampilDataAbsensi()
        Catch ex As Exception
            MessageBox.Show("Gagal memuat data combobox: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' =================== TAMPIL DATA ABSENSI ===================
    Private Sub TampilDataAbsensi()
        Try
            conn.Open()

            Dim sql As String

            ' ================== 1️⃣ KASUS BELUM PILIH KELAS & SEMESTER ==================
            If ComboBox1.Text = "" AndAlso ComboBox2.Text = "" Then
                sql = "
                    SELECT 
                        s.nisn AS 'NISN',
                        s.nama AS 'Nama Siswa',
                        COALESCE(a.total_hadir, 0) AS 'Hadir',
                        COALESCE(a.sakit, 0) AS 'Sakit',
                        COALESCE(a.izin, 0) AS 'Izin',
                        COALESCE(a.alpha, 0) AS 'Alpa'
                    FROM siswa s
                    LEFT JOIN rekap_absensi a ON s.nisn = a.nisn
                    ORDER BY s.nama ASC;"
                Dim da As New MySqlDataAdapter(sql, conn)
                dtAbsensi = New DataTable()
                da.Fill(dtAbsensi)

                ' ================== 2️⃣ KASUS PILIH KELAS SAJA ==================
            ElseIf ComboBox1.Text <> "" AndAlso ComboBox2.Text = "" Then
                sql = "
                    SELECT 
                        s.nisn AS 'NISN',
                        s.nama AS 'Nama Siswa',
                        COALESCE(a.total_hadir, 0) AS 'Hadir',
                        COALESCE(a.sakit, 0) AS 'Sakit',
                        COALESCE(a.izin, 0) AS 'Izin',
                        COALESCE(a.alpha, 0) AS 'Alpa'
                    FROM siswa s
                    INNER JOIN user u ON s.id_guru = u.id_guru
                    LEFT JOIN rekap_absensi a ON s.nisn = a.nisn
                    WHERE u.kelas = @kelas
                    ORDER BY s.nama ASC;"
                Dim da As New MySqlDataAdapter(sql, conn)
                da.SelectCommand.Parameters.AddWithValue("@kelas", ComboBox1.Text)
                dtAbsensi = New DataTable()
                da.Fill(dtAbsensi)

                ' ================== 3️⃣ KASUS PILIH KELAS DAN SEMESTER ==================
            Else
                sql = "
                    SELECT 
                        s.nisn AS 'NISN',
                        s.nama AS 'Nama Siswa',
                        COALESCE(a.total_hadir, 0) AS 'Hadir',
                        COALESCE(a.sakit, 0) AS 'Sakit',
                        COALESCE(a.izin, 0) AS 'Izin',
                        COALESCE(a.alpha, 0) AS 'Alpa'
                    FROM siswa s
                    INNER JOIN user u ON s.id_guru = u.id_guru
                    LEFT JOIN rekap_absensi a 
                        ON s.nisn = a.nisn 
                        AND a.semester = @semester
                    WHERE u.kelas = @kelas
                    ORDER BY s.nama ASC;"
                Dim da As New MySqlDataAdapter(sql, conn)
                da.SelectCommand.Parameters.AddWithValue("@kelas", ComboBox1.Text)
                da.SelectCommand.Parameters.AddWithValue("@semester", ComboBox2.Text)
                dtAbsensi = New DataTable()
                da.Fill(dtAbsensi)
            End If

            ' ================== TAMPILKAN DI DATAGRID ==================
            dvAbsensi = New DataView(dtAbsensi)
            DataGridView1.DataSource = dvAbsensi

            ' === Format tampilan grid ===
            With DataGridView1
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                .DefaultCellStyle.Font = New Font("Segoe UI", 10)
                .ColumnHeadersDefaultCellStyle.BackColor = Color.Navy
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .EnableHeadersVisualStyles = False
                .AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .ReadOnly = True
            End With

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data absensi: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' =================== EVENT COMBOBOX ===================
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        TampilDataAbsensi()
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox1.Text <> "" Then
            TampilDataAbsensi()
        End If
    End Sub

    ' =================== NAVIGASI SIDEBAR ===================
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form19.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form22.Show()
        Me.Hide()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show()
        Me.Hide()
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
