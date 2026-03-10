Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button
Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO

Public Class Form2
    Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
    Dim dtNilai As DataTable
    Dim dvNilai As DataView

    '================= REFRESH DATA =================
    Public Sub RefreshData()
        TampilDataSiswa()
    End Sub

    '================= LOAD FORM =================
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

            ' === Isi ComboBox Mapel ===
            conn.Open()
            Dim cmdMapel As New MySqlCommand("SELECT DISTINCT nama_mapel FROM mata_pelajaran ORDER BY nama_mapel", conn)
            Dim rd2 = cmdMapel.ExecuteReader()
            While rd2.Read()
                ComboBox3.Items.Add(rd2("nama_mapel").ToString())
            End While
            rd2.Close()

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data combobox: " & ex.Message)
        Finally
            conn.Close()
        End Try

        ' tampilkan semua siswa saat awal
        TampilDataSiswa()
    End Sub

    '================= TAMPIL DATA SISWA =================
    Private Sub TampilDataSiswa()
        Try
            conn.Open()

            ' Ambil id_guru dari kelas terpilih
            Dim idGuru As Integer = -1
            If ComboBox1.Text <> "" Then
                Dim cmdGuru As New MySqlCommand("SELECT id_guru FROM user WHERE kelas=@kelas", conn)
                cmdGuru.Parameters.AddWithValue("@kelas", ComboBox1.Text)
                Dim result = cmdGuru.ExecuteScalar()
                If result IsNot Nothing Then idGuru = Convert.ToInt32(result)
            End If

            Dim sql As String

            ' === LOGIKA DIBALIK ===
            If ComboBox3.Text <> "" Then
                ' Jika MAPEL dipilih → tampilkan UH1–UH4 dan UTS
                sql = "
                SELECT s.nisn AS 'NISN', s.nama AS 'Nama',
                       IFNULL(n.nilai_uh1, '-') AS 'UH1',
                       IFNULL(n.nilai_uh2, '-') AS 'UH2',
                       IFNULL(n.nilai_uh3, '-') AS 'UH3',
                       IFNULL(n.nilai_uh4, '-') AS 'UH4',
                       IFNULL(n.nilai_uts, '-') AS 'UTS',
                       IFNULL(n.nilai_sikap, '-') AS 'Sikap'
                FROM siswa s
                LEFT JOIN nilai n ON s.nisn = n.nisn
                LEFT JOIN mata_pelajaran m ON n.id_mapel = m.id_mapel
                WHERE (@id_guru = -1 OR s.id_guru = @id_guru)
                  AND (@semester = '' OR n.semester = @semester)
                  AND m.nama_mapel = @mapel
                GROUP BY s.nisn, s.nama;"
            Else
                ' Jika MAPEL belum dipilih → tampilkan REKAP SEMUA MAPEL
                sql = "
                SELECT s.nisn AS 'NISN', s.nama AS 'Nama',
                     ROUND(AVG(CASE WHEN m.nama_mapel = 'Pendidikan Pancasila dan Kewarganegaraan (PPKn)' THEN n.rata_rata END), 2) AS 'PPKn',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Bahasa Indonesia' THEN n.rata_rata END), 2) AS 'B. Indonesia',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Matematika' THEN n.rata_rata END), 2) AS 'Matematika',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Ilmu Pengetahuan Alam dan Sosial (IPAS)' THEN n.rata_rata END), 2) AS 'IPAS',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Seni Budaya dan Keterampilan (SBK)' THEN n.rata_rata END), 2) AS 'SBK',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Pendidikan Jasmani, Olahraga, dan Kesehatan (PJOK)' THEN n.rata_rata END), 2) AS 'PJOK',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Pendidikan Agama dan Budi Pekerti' THEN n.rata_rata END), 2) AS 'Agama',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Bahasa Inggris' THEN n.rata_rata END), 2) AS 'B. Inggris',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Bahasa Jawa' THEN n.rata_rata END), 2) AS 'B. Jawa',
        ROUND(AVG(n.rata_rata), 2) AS 'Rata-rata Total'
                FROM siswa s
                LEFT JOIN nilai n ON s.nisn = n.nisn
                LEFT JOIN mata_pelajaran m ON n.id_mapel = m.id_mapel
                WHERE (@id_guru = -1 OR s.id_guru = @id_guru)
                  AND (@semester = '' OR n.semester = @semester)
                GROUP BY s.nisn, s.nama;"
            End If

            Dim da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@id_guru", idGuru)
            da.SelectCommand.Parameters.AddWithValue("@semester", ComboBox2.Text)
            da.SelectCommand.Parameters.AddWithValue("@mapel", ComboBox3.Text)
            dtNilai = New DataTable()
            da.Fill(dtNilai)

            dvNilai = New DataView(dtNilai)
            DataGridView1.DataSource = dvNilai

            ' === FORMAT GRID ===
            With DataGridView1
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                .ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
                .DefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10)
                .ColumnHeadersDefaultCellStyle.BackColor = Color.Navy
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .EnableHeadersVisualStyles = False
                .AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .ReadOnly = True
            End With

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data nilai: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub


    '================= FILTER EVENT =================
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox2.Text <> "" Then TampilDataSiswa()
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox1.Text <> "" Then TampilDataSiswa()
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        If ComboBox1.Text <> "" And ComboBox2.Text <> "" Then TampilDataSiswa()
    End Sub

    '================= SIDEBAR NAVIGATION =================
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.Show() : Me.Hide()
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Form19.Show() : Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form22.Show() : Me.Hide()
    End Sub

    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        Form17.Show() : Me.Hide()
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show() : Me.Hide()
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

    '================= EKSPOR PDF =================
    Private Sub Button8_Click_1(sender As Object, e As EventArgs) Handles Button8.Click
        Try
            If DataGridView1.Rows.Count = 0 Then
                MessageBox.Show("Tidak ada data yang dapat diekspor.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim folderPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            Dim namaFile As String = $"Rekap_Nilai_{ComboBox1.Text}_Smt{ComboBox2.Text}_{Now:yyyyMMddHHmmss}.pdf"
            Dim filePath As String = Path.Combine(folderPath, namaFile)

            Dim doc As New Document(PageSize.A4, 40, 40, 40, 40)
            PdfWriter.GetInstance(doc, New FileStream(filePath, FileMode.Create))
            doc.Open()

            Dim titleFont As iTextSharp.text.Font = iTextSharp.text.FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)
            Dim infoFont As iTextSharp.text.Font = iTextSharp.text.FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.NORMAL)
            Dim headerFont As iTextSharp.text.Font = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)
            Dim cellFont As iTextSharp.text.Font = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)

            Dim title As New iTextSharp.text.Paragraph("REKAP NILAI SISWA" & vbCrLf, titleFont)
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER
            doc.Add(title)

            Dim info As New iTextSharp.text.Paragraph(
                $"Kelas: {ComboBox1.Text}     Semester: {ComboBox2.Text}" &
                If(ComboBox3.Text <> "", $"     Mata Pelajaran: {ComboBox3.Text}", "") &
                vbCrLf & vbCrLf, infoFont)
            doc.Add(info)

            Dim pdfTable As New iTextSharp.text.pdf.PdfPTable(DataGridView1.Columns.Count)
            pdfTable.WidthPercentage = 100

            ' Header kolom
            For Each column As DataGridViewColumn In DataGridView1.Columns
                Dim headerCell As New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(column.HeaderText, headerFont))
                headerCell.BackgroundColor = New iTextSharp.text.BaseColor(0, 51, 102)
                headerCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER
                headerCell.Phrase.Font.Color = iTextSharp.text.BaseColor.WHITE
                pdfTable.AddCell(headerCell)
            Next

            ' Isi data
            For Each row As DataGridViewRow In DataGridView1.Rows
                If Not row.IsNewRow Then
                    For Each cell As DataGridViewCell In row.Cells
                        pdfTable.AddCell(New iTextSharp.text.Phrase(If(cell.Value IsNot Nothing, cell.Value.ToString(), ""), cellFont))
                    Next
                End If
            Next

            doc.Add(pdfTable)
            doc.Close()

            MessageBox.Show($"Data berhasil diekspor ke PDF:{vbCrLf}{filePath}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Gagal mengekspor PDF: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
