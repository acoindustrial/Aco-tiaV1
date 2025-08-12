import { IsNumber, IsOptional, IsString, Min } from 'class-validator';

export class StockDto {
  @IsString()
  cod: string;

  @IsNumber()
  @Min(0.0000001)
  cantitate: number;

  @IsOptional()
  @IsString()
  motiv?: string;
}
