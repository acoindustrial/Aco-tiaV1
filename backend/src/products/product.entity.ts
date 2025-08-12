import { Entity, Column, PrimaryColumn } from 'typeorm';

@Entity('products')
export class Product {
  @PrimaryColumn()
  cod: string;

  @Column()
  denumire: string;

  @Column({ type: 'numeric', default: 0 })
  cantitate: number;

  @Column({ length: 3 })
  pozitie: string;

  @Column({ length: 8, default: 'buc' })
  um: string;

  @Column({ default: true })
  is_active: boolean;
}
