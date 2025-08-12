INSERT INTO products (cod, denumire, cantitate, pozitie)
VALUES
  ('ABC123', 'Produs ABC', 10, 'A01'),
  ('XYZ789', 'Produs XYZ', 5, 'B02'),
  ('DEF456', 'Produs DEF', 0, 'C03')
ON CONFLICT DO NOTHING;
