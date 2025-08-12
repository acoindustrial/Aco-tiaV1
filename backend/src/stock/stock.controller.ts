import { Body, Controller, Post, UsePipes, ValidationPipe } from '@nestjs/common';
import { StockService } from './stock.service';
import { StockDto } from './dto/stock.dto';

@Controller('stock')
export class StockController {
  constructor(private readonly stock: StockService) {}

  @Post('in')
  @UsePipes(new ValidationPipe({ whitelist: true }))
  stockIn(@Body() dto: StockDto) {
    return this.stock.stockIn(dto);
  }

  @Post('out')
  @UsePipes(new ValidationPipe({ whitelist: true }))
  stockOut(@Body() dto: StockDto) {
    return this.stock.stockOut(dto);
  }
}
