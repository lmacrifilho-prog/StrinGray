using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StrinGray {
    public partial class MainWindow : Window {

        private readonly Button[] _botoesMenu; // Lista de botões do menu lateral para facilitar iterações

        private string _campoDataAtual; // Indica qual campo de data está sendo editado: "Inicial" ou "Final"

        private DateTime? _dataInicial; // Período selecionado pelo usuário
        private DateTime? _dataFinal;

        private bool _atualizandoCalendario; // Flag para atualizar o calendário

        public MainWindow() { // Inicializa a janela e registra os botões do menu
            InitializeComponent();

            _botoesMenu = new[] {
                BotaoDashboard,
                BotaoAlunos,
                BotaoProfessores,
                BotaoTurmas,
                BotaoHorarios,
                BotaoMatriculas,
                BotaoPagamentos,
                BotaoPresenca,
                BotaoRelatorios,
                BotaoConfiguracoes
            }.Where(b => b != null).ToArray();
        }

        private void Menu_Click(object sender, RoutedEventArgs e) { // Trata o clique no menu lateral exibindo a respectiva área
            if ( !(sender is Button botao) ){
                return;
            }

            
            foreach (Button item in _botoesMenu) { // Remove estilo de selecionado de todos os botões
                if (item == null) continue;
                var defaultStyle = TryFindResource("MenuLateralButton") as Style;
                if (defaultStyle != null) item.Style = defaultStyle;
            }

            var selectedStyle = TryFindResource("MenuLateralButtonSelecionado") as Style;
            if (selectedStyle != null) botao.Style = selectedStyle; // Aplica estilo no botão atualmente clicado

            OcultarPaginas();

            // Se a tag for nula, atribui uma string vazia para evitar erros de referência nula. Caso contrário, converte a tag para string
            string tag = botao.Tag == null ? string.Empty : botao.Tag.ToString();

            switch (tag) {
                case "Dashboard":     AreaDashboard.Visibility     = Visibility.Visible; break;
                case "Alunos":        AreaAlunos.Visibility        = Visibility.Visible; break;
                case "Professores":   AreaProfessores.Visibility   = Visibility.Visible; break;
                case "Turmas":        AreaTurmas.Visibility        = Visibility.Visible; break;
                case "Horários":      AreaHorarios.Visibility      = Visibility.Visible; break;
                case "Matrículas":    AreaMatriculas.Visibility    = Visibility.Visible; break;
                case "Pagamentos":    AreaPagamentos.Visibility    = Visibility.Visible; break;
                case "Presença":      AreaPresenca.Visibility      = Visibility.Visible; break;
                case "Relatórios":    AreaRelatorios.Visibility    = Visibility.Visible; break;
                case "Configurações": AreaConfiguracoes.Visibility = Visibility.Visible; break;
            }
        }

        private void CampoAluno_TextChanged(object sender, TextChangedEventArgs e) { // Exibição o placeholder do campo do aluno

            TextoAlunoPlaceholder.Visibility = string.IsNullOrEmpty(CampoAluno.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void BotaoCalendario_Click(object sender, RoutedEventArgs e) { // Abre o popup de seleção de período
            PopupPeriodo.IsOpen = true;
        }

        private void BotaoDataInicial_Click(object sender, RoutedEventArgs e) { // Inicia a edição da data inicial
            _campoDataAtual = "Inicial";
            _atualizandoCalendario = true;
            CalendarioPeriodo.SelectedDate = _dataInicial;
            _atualizandoCalendario = false;
            CalendarioPeriodo.Visibility = Visibility.Visible;
            ColunaCalendario.Width = new GridLength(232);
            BorderPeriodo.Width = 474;
        }

        private void BotaoDataFinal_Click(object sender, RoutedEventArgs e) { // Inicia a edição da data final
            _campoDataAtual = "Final";
            _atualizandoCalendario = true;
            CalendarioPeriodo.SelectedDate = _dataFinal;
            _atualizandoCalendario = false;
            CalendarioPeriodo.Visibility = Visibility.Visible;
            ColunaCalendario.Width = new GridLength(232);
            BorderPeriodo.Width = 430;
        }

        // Atualiza as datas selecionadas a partir do calendário
        private void CalendarioPeriodo_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
            if (_atualizandoCalendario || !CalendarioPeriodo.SelectedDate.HasValue || string.IsNullOrEmpty(_campoDataAtual))
                return;

            DateTime selecionada = CalendarioPeriodo.SelectedDate.Value;

            if (_campoDataAtual == "Inicial") {
                _dataInicial = selecionada;
                if (TextoDataInicial != null) TextoDataInicial.Text = selecionada.ToString("dd/MM/yyyy");
                var brush = TryFindResource("TextoPeriodoSelecionado") as System.Windows.Media.Brush;
                if (brush != null && TextoDataInicial != null) TextoDataInicial.Foreground = brush;
            } else {
                _dataFinal = selecionada;
                if (TextoDataFinal != null) TextoDataFinal.Text = selecionada.ToString("dd/MM/yyyy");
                var brush = TryFindResource("TextoPeriodoSelecionado") as System.Windows.Media.Brush;
                if (brush != null && TextoDataFinal != null) TextoDataFinal.Foreground = brush;
            }

            // Fecha o calendário e reseta a coluna dedicada
            CalendarioPeriodo.Visibility = Visibility.Collapsed;
            ColunaCalendario.Width = new GridLength(0);
            BorderPeriodo.Width = 230;
            _campoDataAtual = null;
        }

        private void AplicarPeriodo_Click(object sender, RoutedEventArgs e) { // Aplica e atualiza o período selecionado
            // Validação: fim não pode ser anterior ao início
            if (_dataInicial.HasValue && _dataFinal.HasValue && _dataFinal.Value < _dataInicial.Value) {
                MessageBox.Show(
                    "A data final não pode ser anterior à data inicial.",
                    "Período inválido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (_dataInicial.HasValue && _dataFinal.HasValue) {
                TextoPeriodo.Text = string.Format(
                    "{0:dd/MM/yyyy} - {1:dd/MM/yyyy}",
                    _dataInicial.Value,
                    _dataFinal.Value);
                TextoPeriodo.Foreground = (System.Windows.Media.Brush)FindResource("TextoPeriodoSelecionado");

            } else if (_dataInicial.HasValue) {
                TextoPeriodo.Text = string.Format("A partir de {0:dd/MM/yyyy}", _dataInicial.Value);
                TextoPeriodo.Foreground = (System.Windows.Media.Brush)FindResource("TextoPeriodoSelecionado");

            } else if (_dataFinal.HasValue) {
                TextoPeriodo.Text = string.Format("Até {0:dd/MM/yyyy}", _dataFinal.Value);
                TextoPeriodo.Foreground = (System.Windows.Media.Brush)FindResource("TextoPeriodoSelecionado");
            }

            if (CalendarioPeriodo != null) CalendarioPeriodo.Visibility = Visibility.Collapsed;
            if (ColunaCalendario != null) ColunaCalendario.Width = new GridLength(0);
            if (BorderPeriodo != null) BorderPeriodo.Width = 230;
            PopupPeriodo.IsOpen = false;
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void OcultarPaginas() {
            AreaDashboard.Visibility     = Visibility.Collapsed;
            AreaAlunos.Visibility        = Visibility.Collapsed;
            AreaProfessores.Visibility   = Visibility.Collapsed;
            AreaTurmas.Visibility        = Visibility.Collapsed;
            AreaHorarios.Visibility      = Visibility.Collapsed;
            AreaMatriculas.Visibility    = Visibility.Collapsed;
            AreaPagamentos.Visibility    = Visibility.Collapsed;
            AreaPresenca.Visibility      = Visibility.Collapsed;
            AreaRelatorios.Visibility    = Visibility.Collapsed;
            AreaConfiguracoes.Visibility = Visibility.Collapsed;
        }
    }
}