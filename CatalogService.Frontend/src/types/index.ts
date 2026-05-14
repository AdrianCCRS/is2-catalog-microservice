export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: string;
  images: string[];
  tags: string[];
  createdBy: string;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  imageUrl?: string;
  parentId?: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  productCount: number;
}

export interface CartItem {
  productId: string;
  product: Product;
  quantity: number;
}

export interface PaginatedResponse<T> {
  page: number;
  pageSize: number;
  total?: number;
  results: T[];
  search?: string;
}

export interface FilterParams {
  search?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  page: number;
  pageSize: number;
  sortBy?: 'name' | 'price' | 'newest';
  sortOrder?: 'asc' | 'desc';
}
