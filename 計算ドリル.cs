using System;
using System.Windows.Forms;

// 算数ドリルを作ろう
// http://www.greenowl5.com/gprogram/index.html

namespace program_study2
{
    public partial class Form1 : Form
    {        
        Random rand = new Random();
        long noA = 0;       // 問題で使う数字
        long noB = 0;       // 問題で使う数字
        long sumAB = 0;     // 答え
        long mondaiCnt = 0; // 出題数のカウント
        long mondaiMax = 0; // 最大出題数
        long seikaiCnt = 0; // 正解数
        long type = 0;      // 1：足し算　2：引き算　3：掛け算　4：割り算
        string typeStr;     // 演算記号表示用        
        double ansRate = 0; // 正解率
        private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                            // ストップウォッチを生成

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ロード時の初期フォーカスを「開始ボタン」に設定
            this.ActiveControl = this.StartBtn;
            InBox.Enabled = false;  // InBox を無効に
            var sw = new System.Diagnostics.Stopwatch();
        }

        // 開始ボタンを押したときの処理
        private void StartBtn_Click(object sender, EventArgs e)
        {
            // 出題数を変更する
            if (RB10.Checked)
                mondaiMax = 10;
            if (RB20.Checked)
                mondaiMax = 20;
            if (RB30.Checked)
                mondaiMax = 30;
            
            // 計算タイプを変更する
            if (RBtasu.Checked)
            {
                type = 1;
                typeStr = " + ";
            }
            if (RBhiku.Checked)
            {
                type = 2;
                typeStr = " - ";
            }
            if (RBkakeru.Checked)
            {
                type = 3;
                typeStr = " × ";
            }
            if (RBwaru.Checked)
            {
                type = 4;
                typeStr = " ÷ ";
            }

            mondaiCnt = 0;              // 問題数のカウント初期化
            seikaiCnt = 0;              // 正解数のカウント初期化
            InBox.Enabled = true;       // InBoxを有効に。フォーカスより前はないとフォーカスできない            
            MondaiSakusei();
            InBox.Focus();              // 解答欄へフォーカスを移す    
            OutBox.Text = "";
            StartBtn.Text = "リセット";

            sw.Reset();                 // 計測初期化
            OutBoxFinishTime.Text = ""; // FinishTimeLbl の初期化
            sw.Start();                 // 計測開始
            timer1.Enabled = true;      // 経過時間表示の繰り返し処理開始
            timer1.Interval = 500;      // 0.5秒ごとに処理を行う
        }//StartBtn_Click

        // 問題を作成
        private void MondaiSakusei()
        {
            mondaiCnt++;
            MondaiCntLbl.Text = mondaiCnt + " 問目";
            noA = rand.Next(1, 10);
            noB = rand.Next(1, 10);
            MondaiLbl.Text = noA + typeStr + noB + " = ";
            if (type == 1)
                sumAB = noA + noB;
            if (type == 2)
                sumAB = noA - noB;
            if (type == 3)
                sumAB = noA * noB;
            if (type == 4)
                sumAB = noA / noB;
        }//MondaiSakusei

        // キー入力をした時の処理
        private void InBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // e.keychar = 押されたキーを取得
            // InBox.Text = テキストボックスに入力された内容
            // ＊入力を確定した時初めて InBox に入る＊
            
            // 以下「入力を無視する」ものを判別する if 文
            // 入力としての数値ではなく、数字入力そのものを扱う時は、文字コードを使うと良い
            // 左：文字コード０より小さいか、９より大きい
            // 中：バックスペース以外
            // 右：小数点以外
            if ( (e.KeyChar < '0' || e.KeyChar > '9')  &&  (e.KeyChar != '\b') && (e.KeyChar != '.'))
            {
                // (入力された文字がマイナス、かつ、１回目の入力のとき）                
                // このカッコに！をつけて否定している
                // → マイナスは一文字目しか入力できないようにすると同義
                if ( !(e.KeyChar == '-' && InBox.TextLength == 0) )
                {
                    // 小数点は最初の一つしか打てないようにする
                    if ( !(e.KeyChar == '.' && InBox.TextLength == 1) )
                        e.Handled = true;   // いま入力されたものを無視する
                }
            }

            // 「0.」で Enter をできないようにする
            if(e.KeyChar == (char)Keys.Enter)
            {
                if( InBox.Text == "0." )
                {
                    InBox.Text = "";
                    MessageBox.Show("入力が正しくありません");
                }

            }

            // 答え合わせを「行う」条件式            
            // 左 入力が Enter と等しい（Enterを押した）
            // 中 インボックスのテキストが空欄じゃない
            // 右 インボックスのテキストが "-" のみじゃない
            if (e.KeyChar == (char)Keys.Enter && InBox.Text != "" && InBox.Text != "-")
            {
                Console.WriteLine(InBox.Text);              // コンソールに表示
                //OutBox.AppendText(InBox.Text + "\r\n");   // outbox にどんどん追加していく
                                                            // \r\n = 改行コード
                Kotaeawase();
                InBox.Text = "";                            // InBoxのクリア
                
                //問題数が MAX じゃなければ、次の問題を作成する
                if ( mondaiCnt < mondaiMax )
                {
                    MondaiSakusei();
                }
                else
                {
                    Seiseki();
                }
            }
        }//InBox_KeyPress

        // 答え合わせ
        private void Kotaeawase()
        {
            // Inr64.parse() = ()内の取得した値を数値型に変換する
            if ( sumAB == Int64.Parse(InBox.Text))
            {                
                OutBox.AppendText("〇 ");    // ここで \r\n を入力しないのがポイント
                seikaiCnt++;                 // 正解数を+1
            }
            else
            {
                // ここで \r\n を入力しないのがポイント
                OutBox.AppendText("× ");
            }

            // 問題 + 入力 + 改行
            OutBox.AppendText(MondaiLbl.Text + InBox.Text + "\r\n");
        }//Kotaeawase

        // 成績処理
        private void Seiseki()
        {
            mondaiCnt++;                // Timer1_Tick を終了するためのプラス１
            InBox.Enabled = false;      // InBox を無効に
            MondaiLbl.Text = "";        // 問題の表示のクリア
            OutBox.AppendText("正解数は　" + seikaiCnt + "　です");

            // 正解率の表示
            ansRate = 1.0 * seikaiCnt / mondaiMax * 100;    // C#の場合、整数同士ではなく実数同士の割り算であることを示すために、1.0 を最初にかける必要がある。
            OutBoxCorrectAnsRate.Text = ansRate + " %";
        }//Seiseki

        // 経過時間を１秒ごとに表示する
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (mondaiCnt <= mondaiMax)
            {
                //Console.WriteLine(mondaiCnt);
                //Console.WriteLine(mondaiMax);
                OutBoxTime.Text = sw.Elapsed.ToString(@"hh\:mm\:ss");        // 経過時間を表示
            }
            else
            {
                timer1.Enabled = false;                         // 経過時間表示の繰り返しを終了
                OutBoxTime.Text = "";
                OutBoxFinishTime.Text = sw.Elapsed.ToString(@"hh\:mm\:ss");  // 最終的なタイムを表示
            }
        }//Timer1_Tick

        private void InBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void OutBox_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void label1Ans_Click(object sender, EventArgs e)
        {

        }

        private void CorrectAnsRateLbl_Click(object sender, EventArgs e)
        {

        }

        private void TimeLbl_Click(object sender, EventArgs e)
        {

        }

        private void FinishTimeLbl_Click(object sender, EventArgs e)
        {

        }

        private void OutBoxTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void MondaiLbl_Click(object sender, EventArgs e)
        {

        }
    }
}
