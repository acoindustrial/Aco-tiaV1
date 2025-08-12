import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository, ILike } from 'typeorm';
import { Product } from './product.entity';

@Injectable()
export class ProductsService {
  constructor(@InjectRepository(Product) private repo: Repository<Product>) {}

  async searchByCod(cod: string): Promise<Product[]> {
    if (cod.endsWith('*')) {
      const prefix = cod.slice(0, -1);
      return this.repo.find({ where: { cod: ILike(`${prefix}%`) } });
    }
    const product = await this.repo.findOne({ where: { cod } });
    return product ? [product] : [];
  }

  async getByCod(cod: string): Promise<Product> {
    const product = await this.repo.findOne({ where: { cod } });
    if (!product) {
      throw new NotFoundException('Produs inexistent');
    }
    return product;
  }

  async save(product: Product) {
    return this.repo.save(product);
  }
}
