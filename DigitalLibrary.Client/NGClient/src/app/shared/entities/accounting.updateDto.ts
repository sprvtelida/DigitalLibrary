export class AccountingForUpdateDto {
  constructor(id: string, status: number) {
    this.id = id;
    this.status = status;
  }

  id: string;
  status: number;
}
