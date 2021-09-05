
export class BookParameters {
        pageSize: number = 4; 
        searchTerm?: string; 
        minYear?:number;
        maxYear?:number; 
        minPages?:number; 
        maxPages?:number; 
        onlyInStorage?:boolean;
        orderBy?:string; 
        searchField?: string;
        genreIds?: string[];
        subjectIds?: string[]; 
        pageNumber?: number;
        libraryId?: string;

        constructor(pageSize?, genreIds?, subjectIds?, searchTerm?, minYear?, maxYear?, minPages?, maxPages?, onlyInStorage?, orderBy?, searchField?) {
                this.pageSize = pageSize,
                this.genreIds = genreIds,
                this.subjectIds = subjectIds,
                this.searchTerm = searchTerm,
                this.minYear = minYear,
                this.maxYear = maxYear,
                this.minPages = minPages,
                this.maxPages = maxPages,
                this.onlyInStorage = onlyInStorage,
                this.orderBy = orderBy,
                this.searchField = searchField
        }
}