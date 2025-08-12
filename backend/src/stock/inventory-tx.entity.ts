import { Entity, PrimaryGeneratedColumn, ManyToOne, Column, CreateDateColumn } from 'typeorm';
import { Product } from '../products/product.entity';

@Entity('inventory_tx')
export class InventoryTx {
  @PrimaryGeneratedColumn()
  id: number;

  @ManyToOne(() => Product, { eager: true })
  product: Product;

  @Column({ length: 3 })
  tip: string; // IN/OUT/ADJ

  @Column({ type: 'numeric' })
  cantitate: number;

  @Column({ nullable: true })
  motiv?: string;

  @CreateDateColumn({ type: 'timestamp' })
  created_at: Date;
}
