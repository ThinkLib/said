import { default as CategoryDb, CategoryModel, ICategory } from '../models/category'
import { Log } from '../utils/log'
import * as jwt from 'jsonwebtoken'
import { ServiceError } from '../models/server/said-error'
import * as crypto from 'crypto'

const log = new Log('service/category')


/**
 * 查询
 */
export const queryCategoryAll = () => {
  log.info('queryAll.call', null)
  return CategoryDb.find().exec()
}


/**
 * 新增
 */
export const createCategory = (category: ICategory) => {
  log.info('addCategory.call', category)
  const categorydb = new CategoryDb(category)
  return categorydb.save()
}

/**
 * 修改
 * @param category 
 */
export const updateCategoryById = (id: string, category: { icon?: string, name?: string }) => {
  log.info('editCategory.call', { id, category })
  return CategoryDb.findByIdAndUpdate(id, category).exec()
}


/**
 * 删除
 */
export const removeCategory = (id: string) => {
  log.info('removeCategory.call', id)
  return CategoryDb.findByIdAndRemove(id).exec()
}

