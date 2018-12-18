import { Component, OnInit, Input } from '@angular/core';
import { CategoryHierarchy } from './category-hierarchy';
import { Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { ParameterNames } from '../../parameter/parameter-names';
import { ParameterService } from '../../parameter/parameter.service';
import { Router } from '@angular/router';
import { Category } from '../category';
import { CategoryService } from '../category.service';
import { EntityIndex } from 'src/app/models/entity/entity-index';

@Component({
  selector: 'app-category-index',
  templateUrl: './category-index.component.html',
  styleUrls: ['./category-index.component.css'],
})
export class CategoryIndexComponent implements OnInit {
  public allCategories: Category[];
  protected categoryHierarchy: CategoryHierarchy;
  protected currentCategory: Category;
  protected parametersSubsription: Subscription;
  protected categoryIdParamName = ParameterNames.categoryId;
  @Input() administrating: boolean;

  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private parameterService: ParameterService) {
  }

  ngOnInit() {
    this.initializeComponent();
  }

  initializeComponent() {
    this.parametersSubsription = this.parameterService.params$.pipe(
      debounceTime(50))
      .subscribe(
      params => {
        this.updateCategories();
      }
    );
    this.updateCategories();
  }
  updateCategories() {
    this.allCategories = [];
    let categoryId: number = +this.parameterService.getParam(ParameterNames.categoryId);
    if (!categoryId)
      categoryId = this.categoryService.rootCategoryId;
    if (!this.currentCategory || this.currentCategory.id != categoryId) {
      this.categoryService.index(this.parameterService.getUpdatedParams([ParameterNames.categoryId, categoryId])).subscribe((data: EntityIndex<Category>) => {
        data.entities.forEach(c => this.allCategories.push(new Category(c)));
        this.currentCategory = this.allCategories.find(c => c.id == categoryId);
        this.categoryHierarchy = new CategoryHierarchy(this.categoryService.rootCategoryId, this.currentCategory, this.allCategories);
      });
    }
  }
  getCategoryParams(categoryId: number): any {
    let params = this.parameterService.getUpdatedParams(
      [ParameterNames.categoryId, categoryId],
      [ParameterNames.characteristicsFilter, null],
      [ParameterNames.page, null],
      [ParameterNames.characteristicId, null]);
    return params;
  }

  create() {
    let categoryId = this.parameterService.getParam(ParameterNames.categoryId);
    this.router.navigate(['/categories/create'], { queryParams: { categoryId: categoryId } });
  }
}
