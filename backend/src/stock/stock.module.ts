import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Product } from '../products/product.entity';
import { InventoryTx } from './inventory-tx.entity';
import { StockService } from './stock.service';
import { StockController } from './stock.controller';

@Module({
  imports: [TypeOrmModule.forFeature([Product, InventoryTx])],
  providers: [StockService],
  controllers: [StockController],
})
export class StockModule {}
