import { useQuery } from '@tanstack/react-query';
import { apiClient } from '@/services/apiClient';
import { Category } from '@/types';

export const useCategories = () => {
  return useQuery<Category[]>({
    queryKey: ['categories'],
    queryFn: () => apiClient.getCategories(),
    staleTime: 30 * 60 * 1000, // 30 minutos
  });
};
