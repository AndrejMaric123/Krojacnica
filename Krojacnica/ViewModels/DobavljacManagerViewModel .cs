using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Krojacnica.Views
{
    public class DobavljacManagerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DobavljacViewModel> SviDobavljaci { get; set; }
        public ObservableCollection<DobavljacViewModel> Individualni { get; set; }
        public ObservableCollection<DobavljacViewModel> Preduzeca { get; set; }

        public SnackbarMessageQueue SnackbarQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        // Snackbar event
        public event Action<string, bool, Action?> SnackbarMessage;
        // string = poruka
        // bool   = da li je error
        // Action = undo

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged();
                    PrimijeniFilter();
                }
            }
        }

        public DobavljacManagerViewModel()
        {
            SviDobavljaci = new();
            Individualni = new();
            Preduzeca = new();

            LoadFromDatabase();
        }

        private void LoadFromDatabase()
        {
            using var context = DbContextFactory.Create();

            var svi = context.dobavljacs
                .Include(d => d.individualni)
                .Include(d => d.preduzece)
                .Include(d => d.mjesto_postaNavigation)
                .ToList()
                .Select(d => new DobavljacViewModel(d))
                .ToList();

            SviDobavljaci.Clear();
            foreach (var d in svi)
                SviDobavljaci.Add(d);

            PrimijeniFilter();
        }

        private void PrimijeniFilter()
        {
            string query = SearchQuery?.Trim().ToLower() ?? "";

            Individualni.Clear();
            Preduzeca.Clear();

            foreach (var d in SviDobavljaci)
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    if (!d.NazivIliIme.ToLower().Contains(query))
                        continue;
                }

                if (d.Tip == "Individualni")
                    Individualni.Add(d);
                else
                    Preduzeca.Add(d);
            }
        }

        // GLAVNO – Brisanje + Undo
        public bool DeleteDobavljac(int sifra)
        {
            using var context = DbContextFactory.Create();

            var db = context.dobavljacs
                .Include(d => d.individualni)
                .Include(d => d.preduzece)
                .FirstOrDefault(d => d.sifra == sifra);

            if (db == null)
            {
                ShowMessage("Dobavljač ne postoji u bazi.", true);
                return false;
            }

            // Sačuvamo podatke za undo
            var backupDobavljac = new dobavljac
            {
                sifra = db.sifra,
                adresa = db.adresa,
                telefon = db.telefon,
                mjesto_posta = db.mjesto_posta,
                individualni = db.individualni != null
                    ? new individualni
                    {
                        dobavljac_sifra = db.sifra,
                        ime = db.individualni.ime,
                        prezime = db.individualni.prezime,
                        jmb = db.individualni.jmb
                    }
                    : null,
                preduzece = db.preduzece != null
                    ? new preduzece
                    {
                        dobavljac_sifra = db.sifra,
                        naziv = db.preduzece.naziv
                    }
                    : null
            };

            try
            {
                if (db.individualni != null)
                    context.individualnis.Remove(db.individualni);

                if (db.preduzece != null)
                    context.preduzeces.Remove(db.preduzece);

                context.dobavljacs.Remove(db);
                context.SaveChanges();
                Reload();

                // UNDO AKCIJA
                void UndoAction()
                {
                    using var undoCtx = DbContextFactory.Create();
                    undoCtx.dobavljacs.Add(backupDobavljac);

                    if (backupDobavljac.individualni != null)
                        undoCtx.individualnis.Add(backupDobavljac.individualni);

                    if (backupDobavljac.preduzece != null)
                        undoCtx.preduzeces.Add(backupDobavljac.preduzece);

                    undoCtx.SaveChanges();

                    Reload();
                }

                ShowMessage("Dobavljač obrisan.", false, UndoAction);
                return true;
            }
            catch
            {
                ShowMessage("Dobavljača nije moguće obrisati jer se koristi u otkupu.", true, null);
                return false;
            }
        }

        public void Reload()
        {
            LoadFromDatabase();
        }

        private void ShowMessage(string text, bool isError, Action? undo = null)
        {
            SnackbarMessage?.Invoke(text, isError, undo);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
