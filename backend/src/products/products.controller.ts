import { Controller, Get, Param, Query } from '@nestjs/common';
import { ProductsService } from './products.service';

@Controller('products')
export class ProductsController {
  constructor(private readonly products: ProductsService) {}

  @Get()
  search(@Query('cod') cod: string) {
    if (!cod) return [];
    return this.products.searchByCod(cod);
  }

  @Get(':cod')
  getOne(@Param('cod') cod: string) {
    return this.products.getByCod(cod);
  }
}
