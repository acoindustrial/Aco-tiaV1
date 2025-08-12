import { BadRequestException, Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Product } from '../products/product.entity';
import { InventoryTx } from './inventory-tx.entity';
import { StockDto } from './dto/stock.dto';

@Injectable()
export class StockService {
  constructor(
    @InjectRepository(Product) private products: Repository<Product>,
    @InjectRepository(InventoryTx) private txRepo: Repository<InventoryTx>,
  ) {}

  async stockIn(dto: StockDto) {
    const product = await this.products.findOne({ where: { cod: dto.cod } });
    if (!product) throw new NotFoundException('Produs inexistent');
    product.cantitate = Number(product.cantitate) + dto.cantitate;
    await this.products.save(product);
    await this.txRepo.save({ product, tip: 'IN', cantitate: dto.cantitate, motiv: dto.motiv });
    return { ok: true, cod: product.cod, cantitate: product.cantitate };
  }

  async stockOut(dto: StockDto) {
    const product = await this.products.findOne({ where: { cod: dto.cod } });
    if (!product) throw new NotFoundException('Produs inexistent');
    if (Number(product.cantitate) - dto.cantitate < 0)
      throw new BadRequestException('Stoc insuficient');
    product.cantitate = Number(product.cantitate) - dto.cantitate;
    await this.products.save(product);
    await this.txRepo.save({ product, tip: 'OUT', cantitate: dto.cantitate, motiv: dto.motiv });
    return { ok: true, cod: product.cod, cantitate: product.cantitate };
  }
}
