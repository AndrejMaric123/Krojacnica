using System;
using System.Collections.Generic;
using Krojacnica.Models;
using Microsoft.EntityFrameworkCore;

namespace Krojacnica.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<admin> admins { get; set; }

    public virtual DbSet<artikal> artikals { get; set; }

    public virtual DbSet<boja> bojas { get; set; }

    public virtual DbSet<dobavljac> dobavljacs { get; set; }

    public virtual DbSet<individualni> individualnis { get; set; }

    public virtual DbSet<isplatum> isplata { get; set; }

    public virtual DbSet<klijent> klijents { get; set; }

    public virtual DbSet<materijal> materijals { get; set; }

    public virtual DbSet<materijal_dobavljac> materijal_dobavljacs { get; set; }

    public virtual DbSet<materijal_zaliha> materijal_zalihas { get; set; }

    public virtual DbSet<mjere> mjeres { get; set; }

    public virtual DbSet<mjesto> mjestos { get; set; }

    public virtual DbSet<narudzba> narudzbas { get; set; }

    public virtual DbSet<osoba> osobas { get; set; }

    public virtual DbSet<otkup> otkups { get; set; }

    public virtual DbSet<otkup_isplatum> otkup_isplata { get; set; }

    public virtual DbSet<otkup_stavka> otkup_stavkas { get; set; }

    public virtual DbSet<ponudum> ponuda { get; set; }

    public virtual DbSet<preduzece> preduzeces { get; set; }

    public virtual DbSet<proba> probas { get; set; }

    public virtual DbSet<racun> racuns { get; set; }

    public virtual DbSet<status_narudzbe> status_narudzbes { get; set; }

    public virtual DbSet<stavka_narudzbe> stavka_narudzbes { get; set; }

    public virtual DbSet<usluga> uslugas { get; set; }

    public virtual DbSet<zaposleni> zaposlenis { get; set; }

    public virtual DbSet<zaposlenje> zaposlenjes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<admin>(entity =>
        {
            entity.HasKey(e => e.zaposleni_osoba_id).HasName("PRIMARY");

            entity.ToTable("admin");

            entity.Property(e => e.zaposleni_osoba_id).ValueGeneratedNever();

            entity.HasOne(d => d.zaposleni_osoba).WithOne(p => p.admin)
                .HasForeignKey<admin>(d => d.zaposleni_osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("admin_ibfk_1");
        });

        modelBuilder.Entity<artikal>(entity =>
        {
            entity.HasKey(e => e.ponuda_id).HasName("PRIMARY");

            entity.ToTable("artikal");

            entity.HasIndex(e => e.boja_hex_code, "fk_ARTIKAL_BOJA1_idx");

            entity.HasIndex(e => e.materijal_id, "fk_ARTIKAL_MATERIJAL1_idx");

            entity.Property(e => e.ponuda_id).ValueGeneratedNever();
            entity.Property(e => e.boja_hex_code).HasMaxLength(7);
            entity.Property(e => e.naziv).HasMaxLength(255);
            entity.Property(e => e.slika).HasColumnType("mediumblob");

            entity.HasOne(d => d.boja_hex_codeNavigation).WithMany(p => p.artikals)
                .HasForeignKey(d => d.boja_hex_code)
                .HasConstraintName("fk_ARTIKAL_BOJA");

            entity.HasOne(d => d.materijal).WithMany(p => p.artikals)
                .HasForeignKey(d => d.materijal_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("artikal_ibfk_1");

            entity.HasOne(d => d.ponuda).WithOne(p => p.artikal)
                .HasForeignKey<artikal>(d => d.ponuda_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ARTIKAL_PONUDA1");
        });

        modelBuilder.Entity<boja>(entity =>
        {
            entity.HasKey(e => e.hex_code).HasName("PRIMARY");

            entity.ToTable("boja");

            entity.Property(e => e.hex_code).HasMaxLength(7);
            entity.Property(e => e.naziv).HasMaxLength(45);
        });

        modelBuilder.Entity<dobavljac>(entity =>
        {
            entity.HasKey(e => e.sifra).HasName("PRIMARY");

            entity.ToTable("dobavljac");

            entity.HasIndex(e => e.mjesto_posta, "fk_DOBAVLJAČ_MJESTO1_idx");

            entity.Property(e => e.sifra).ValueGeneratedNever();
            entity.Property(e => e.adresa).HasMaxLength(45);
            entity.Property(e => e.telefon).HasMaxLength(45);

            entity.HasOne(d => d.mjesto_postaNavigation).WithMany(p => p.dobavljacs)
                .HasForeignKey(d => d.mjesto_posta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DOBAVLJAČ_MJESTO1");
        });

        modelBuilder.Entity<individualni>(entity =>
        {
            entity.HasKey(e => e.dobavljac_sifra).HasName("PRIMARY");

            entity.ToTable("individualni");

            entity.Property(e => e.dobavljac_sifra).ValueGeneratedNever();
            entity.Property(e => e.ime).HasMaxLength(20);
            entity.Property(e => e.jmb)
                .HasMaxLength(13)
                .IsFixedLength();
            entity.Property(e => e.prezime).HasMaxLength(20);

            entity.HasOne(d => d.dobavljac_sifraNavigation).WithOne(p => p.individualni)
                .HasForeignKey<individualni>(d => d.dobavljac_sifra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_INDIVIDUALNI_DOBAVLJAČ1");
        });

        modelBuilder.Entity<isplatum>(entity =>
        {
            entity.HasKey(e => e.broj_isplate).HasName("PRIMARY");

            entity.HasIndex(e => e.dobavljac_sifra, "fk_ISPLATA_DOBAVLJAČ1_idx");

            entity.Property(e => e.iznos).HasPrecision(8, 2);

            entity.HasOne(d => d.dobavljac_sifraNavigation).WithMany(p => p.isplata)
                .HasForeignKey(d => d.dobavljac_sifra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ISPLATA_DOBAVLJAČ1");
        });

        modelBuilder.Entity<klijent>(entity =>
        {
            entity.HasKey(e => e.osoba_id).HasName("PRIMARY");

            entity.ToTable("klijent");

            entity.Property(e => e.osoba_id).ValueGeneratedNever();

            entity.HasOne(d => d.osoba).WithOne(p => p.klijent)
                .HasForeignKey<klijent>(d => d.osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Klijent_OSOBA");
        });

        modelBuilder.Entity<materijal>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("materijal");

            entity.Property(e => e.kvalitet).HasMaxLength(20);
            entity.Property(e => e.naziv).HasMaxLength(45);
        });

        modelBuilder.Entity<materijal_dobavljac>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity
                .ToTable("materijal_dobavljac")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.dobavljac_sifra, "dobavljac_sifra");

            entity.HasIndex(e => e.materijal_id, "materijal_id");

            entity.Property(e => e.cijena).HasPrecision(6, 2);

            entity.HasOne(d => d.dobavljac_sifraNavigation).WithMany(p => p.materijal_dobavljacs)
                .HasForeignKey(d => d.dobavljac_sifra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("materijal_dobavljac_ibfk_2");

            entity.HasOne(d => d.materijal).WithMany(p => p.materijal_dobavljacs)
                .HasForeignKey(d => d.materijal_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("materijal_dobavljac_ibfk_3");
        });

        modelBuilder.Entity<materijal_zaliha>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("materijal_zaliha");

            entity.HasIndex(e => e.boja_hex_code, "fk_zaliha_boja");

            entity.HasIndex(e => new { e.materijal_id, e.boja_hex_code }, "uq_materijal_boja").IsUnique();

            entity.Property(e => e.boja_hex_code).HasMaxLength(7);

            entity.HasOne(d => d.boja_hex_codeNavigation).WithMany(p => p.materijal_zalihas)
                .HasForeignKey(d => d.boja_hex_code)
                .HasConstraintName("fk_zaliha_boja");

            entity.HasOne(d => d.materijal).WithMany(p => p.materijal_zalihas)
                .HasForeignKey(d => d.materijal_id)
                .HasConstraintName("fk_zaliha_materijal");
        });

        modelBuilder.Entity<mjere>(entity =>
        {
            entity.HasKey(e => new { e.klijent_osoba_id, e.datum })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("mjere");

            entity.HasOne(d => d.klijent_osoba).WithMany(p => p.mjeres)
                .HasForeignKey(d => d.klijent_osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_MJERE_KLIJENT1");
        });

        modelBuilder.Entity<mjesto>(entity =>
        {
            entity.HasKey(e => e.posta).HasName("PRIMARY");

            entity.ToTable("mjesto");

            entity.Property(e => e.posta).ValueGeneratedNever();
            entity.Property(e => e.naziv).HasMaxLength(45);
        });

        modelBuilder.Entity<narudzba>(entity =>
        {
            entity.HasKey(e => e.broj_narudzbe).HasName("PRIMARY");

            entity.ToTable("narudzba");

            entity.HasIndex(e => e.klijent_osoba_id, "fk_NARUDŽBA_KLIJENT1_idx");

            entity.HasIndex(e => e.status_narudzbe_naziv, "fk_NARUDŽBA_STATUS_NARUDZBE1_idx");

            entity.HasIndex(e => e.zaposleni_osoba_id, "fk_NARUDŽBA_ZAPOSLENI1_idx");

            entity.HasOne(d => d.klijent_osoba).WithMany(p => p.narudzbas)
                .HasForeignKey(d => d.klijent_osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NARUDŽBA_KLIJENT1");

            entity.HasOne(d => d.status_narudzbe_nazivNavigation).WithMany(p => p.narudzbas)
                .HasForeignKey(d => d.status_narudzbe_naziv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NARUDŽBA_STATUS_NARUDZBE1");

            entity.HasOne(d => d.zaposleni_osoba).WithMany(p => p.narudzbas)
                .HasForeignKey(d => d.zaposleni_osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NARUDŽBA_ZAPOSLENI1");
        });

        modelBuilder.Entity<osoba>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("osoba");

            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.ime).HasMaxLength(20);
            entity.Property(e => e.prezime).HasMaxLength(20);
            entity.Property(e => e.telefon).HasMaxLength(45);
        });

        modelBuilder.Entity<otkup>(entity =>
        {
            entity.HasKey(e => e.broj_potvrde).HasName("PRIMARY");

            entity.ToTable("otkup");

            entity.HasIndex(e => e.dobavljac_sifra, "fk_OTKUP_DOBAVLJAČ1_idx");

            entity.HasOne(d => d.dobavljac_sifraNavigation).WithMany(p => p.otkups)
                .HasForeignKey(d => d.dobavljac_sifra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_OTKUP_DOBAVLJAČ1");
        });

        modelBuilder.Entity<otkup_isplatum>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.isplata_broj_isplate, "fk_isplata");

            entity.HasIndex(e => e.otkup_broj_potvrde, "fk_otkup");

            entity.HasOne(d => d.isplata_broj_isplateNavigation).WithMany(p => p.otkup_isplata)
                .HasForeignKey(d => d.isplata_broj_isplate)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_isplata");

            entity.HasOne(d => d.otkup_broj_potvrdeNavigation).WithMany(p => p.otkup_isplata)
                .HasForeignKey(d => d.otkup_broj_potvrde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_otkup");
        });

        modelBuilder.Entity<otkup_stavka>(entity =>
        {
            entity.HasKey(e => new { e.boja_hex_code, e.materijal_dobavljac_id })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("otkup_stavka");

            entity.HasIndex(e => e.otkup_broj_potvrde, "fk_OTKUP_STAVKA_OTKUP1_idx");

            entity.HasIndex(e => e.materijal_dobavljac_id, "materijal_dobavljac_id");

            entity.Property(e => e.boja_hex_code).HasMaxLength(7);

            entity.HasOne(d => d.materijal_dobavljac).WithMany(p => p.otkup_stavkas)
                .HasForeignKey(d => d.materijal_dobavljac_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("otkup_stavka_ibfk_1");

            entity.HasOne(d => d.otkup_broj_potvrdeNavigation).WithMany(p => p.otkup_stavkas)
                .HasForeignKey(d => d.otkup_broj_potvrde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("otkup_stavka_ibfk_2");
        });

        modelBuilder.Entity<ponudum>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.id).ValueGeneratedNever();
            entity.Property(e => e.jedinicna_cijena).HasPrecision(6, 2);
        });

        modelBuilder.Entity<preduzece>(entity =>
        {
            entity.HasKey(e => e.dobavljac_sifra).HasName("PRIMARY");

            entity.ToTable("preduzece");

            entity.Property(e => e.dobavljac_sifra).ValueGeneratedNever();
            entity.Property(e => e.jib).HasMaxLength(45);
            entity.Property(e => e.naziv).HasMaxLength(45);

            entity.HasOne(d => d.dobavljac_sifraNavigation).WithOne(p => p.preduzece)
                .HasForeignKey<preduzece>(d => d.dobavljac_sifra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PREDUZEĆE_DOBAVLJAČ1");
        });

        modelBuilder.Entity<proba>(entity =>
        {
            entity.HasKey(e => new { e.stavka_narudzbe_ponuda_id, e.stavka_narudzbe_narudzba_broj_narudzbe, e.datum_probe })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("proba");

            entity.HasIndex(e => new { e.stavka_narudzbe_ponuda_id, e.stavka_narudzbe_narudzba_broj_narudzbe }, "fk_PROBA_STAVKA_NARUDŽBE1_idx");

            entity.Property(e => e.komentar).HasColumnType("text");

            entity.HasOne(d => d.stavka_narudzbe).WithMany(p => p.probas)
                .HasForeignKey(d => new { d.stavka_narudzbe_ponuda_id, d.stavka_narudzbe_narudzba_broj_narudzbe })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PROBA_STAVKA_NARUDŽBE1");
        });

        modelBuilder.Entity<racun>(entity =>
        {
            entity.HasKey(e => e.broj_racuna).HasName("PRIMARY");

            entity.ToTable("racun");

            entity.HasIndex(e => e.klijent_osoba_id, "fk_RAČUN_KLIJENT1_idx");

            entity.HasIndex(e => e.narudzba_broj_narudzbe, "fk_RAČUN_NARUDŽBA1_idx");

            entity.HasIndex(e => e.zaposleni_osoba_id, "fk_RAČUN_ZAPOSLENI1_idx");

            entity.Property(e => e.NačinPlaćanja).HasMaxLength(20);
            entity.Property(e => e.ukupan_iznos).HasPrecision(6, 2);

            entity.HasOne(d => d.klijent_osoba).WithMany(p => p.racuns)
                .HasForeignKey(d => d.klijent_osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RAČUN_KLIJENT1");

            entity.HasOne(d => d.narudzba_broj_narudzbeNavigation).WithMany(p => p.racuns)
                .HasForeignKey(d => d.narudzba_broj_narudzbe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RAČUN_NARUDŽBA1");

            entity.HasOne(d => d.zaposleni_osoba).WithMany(p => p.racuns)
                .HasForeignKey(d => d.zaposleni_osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RAČUN_ZAPOSLENI1");
        });

        modelBuilder.Entity<status_narudzbe>(entity =>
        {
            entity.HasKey(e => e.naziv).HasName("PRIMARY");

            entity.ToTable("status_narudzbe");
        });

        modelBuilder.Entity<stavka_narudzbe>(entity =>
        {
            entity.HasKey(e => new { e.ponuda_id, e.narudzba_broj_narudzbe })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("stavka_narudzbe");

            entity.HasIndex(e => new { e.mjere_klijent_osoba_id, e.mjere_datum }, "fk_STAVKA_NARUDŽBE_MJERE1_idx");

            entity.HasIndex(e => e.narudzba_broj_narudzbe, "fk_STAVKA_NARUDŽBE_NARUDŽBA1_idx");

            entity.Property(e => e.Cijena).HasPrecision(6, 2);

            entity.HasOne(d => d.narudzba_broj_narudzbeNavigation).WithMany(p => p.stavka_narudzbes)
                .HasForeignKey(d => d.narudzba_broj_narudzbe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_STAVKA_NARUDŽBE_NARUDŽBA1");

            entity.HasOne(d => d.ponuda).WithMany(p => p.stavka_narudzbes)
                .HasForeignKey(d => d.ponuda_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_STAVKA_NARUDŽBE_PONUDA1");

            entity.HasOne(d => d.mjere).WithMany(p => p.stavka_narudzbes)
                .HasForeignKey(d => new { d.mjere_klijent_osoba_id, d.mjere_datum })
                .HasConstraintName("fk_STAVKA_NARUDŽBE_MJERE1");
        });

        modelBuilder.Entity<usluga>(entity =>
        {
            entity.HasKey(e => e.ponuda_id).HasName("PRIMARY");

            entity.ToTable("usluga");

            entity.Property(e => e.ponuda_id).ValueGeneratedNever();
            entity.Property(e => e.naziv).HasMaxLength(45);

            entity.HasOne(d => d.ponuda).WithOne(p => p.usluga)
                .HasForeignKey<usluga>(d => d.ponuda_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_USLUGA_PONUDA1");
        });

        modelBuilder.Entity<zaposleni>(entity =>
        {
            entity.HasKey(e => e.osoba_id).HasName("PRIMARY");

            entity.ToTable("zaposleni");

            entity.Property(e => e.osoba_id).ValueGeneratedNever();
            entity.Property(e => e.korisnicko_ime).HasMaxLength(45);
            entity.Property(e => e.lozinka).HasMaxLength(255);

            entity.HasOne(d => d.osoba).WithOne(p => p.zaposleni)
                .HasForeignKey<zaposleni>(d => d.osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Zaposleni_OSOBA1");
        });

        modelBuilder.Entity<zaposlenje>(entity =>
        {
            entity.HasKey(e => e.broj_ugovora).HasName("PRIMARY");

            entity.ToTable("zaposlenje");

            entity.HasIndex(e => e.osoba_id, "fk_ZAPOSLENJE_ZAPOSLENI1_idx");

            entity.HasOne(d => d.osoba).WithMany(p => p.zaposlenjes)
                .HasForeignKey(d => d.osoba_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ZAPOSLENJE_ZAPOSLENI1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
